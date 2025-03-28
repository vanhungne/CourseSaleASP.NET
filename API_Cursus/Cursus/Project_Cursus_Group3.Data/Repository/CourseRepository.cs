using AutoMapper;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model;
using Project_Cursus_Group3.Data.Model.CourseModel;
using Project_Cursus_Group3.Data.Model.ReasonModel;
using Project_Cursus_Group3.Data.ViewModels.CourseDTO;
using System.Runtime.InteropServices;

namespace Project_Cursus_Group3.Data.Repository
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {

        private readonly IMapper _map;
        private readonly CursusDbContext _dbContext;
        private readonly IConfiguration configuration;
        private readonly IReasonRepository reasonRepository;


        public CourseRepository(CursusDbContext context, IMapper mapper, IConfiguration configuration) : base(context)
        {
            _dbContext = context;
            this.configuration = configuration;
            _map = mapper;
        }

        public Task<Course> AddAsync(Course course)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Course>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Course> DeleteAsync(int id, string reasonContent)
        {
            var course = await GetByIdAsync(id);
            if (course == null)
            {
                throw new KeyNotFoundException($"Course with Id {id} not found.");
            }

            course.Status = "Pending";

            var reason = new Reason
            {
                Content = reasonContent,
                Status = "Pending",
                CourseId = course.CourseId
            };

            _dbContext.Reason.Add(reason);
            await _dbContext.SaveChangesAsync();

            return course;
        }


        public async Task<IEnumerable<CourseViewGET>> GetAllCourseActiveAsync(CourseSearchOptions searchOptions)
        {
            var query = Entities.Include(c => c.Category)
                                .Include(c => c.User)
                                .Include(c => c.PurchasedCourses)
                                     .ThenInclude(pc => pc.Feedback)
                                .Where(c => c.Status.ToLower() == "active");
            if (!string.IsNullOrEmpty(searchOptions.CourseCode))
            {
                query = query.Where(c => c.CourseCode.Contains(searchOptions.CourseCode));
            }
            if (!string.IsNullOrEmpty(searchOptions.CourseTitle))
            {
                query = query.Where(c => c.CourseTitle.Contains(searchOptions.CourseTitle));
            }

            if (!string.IsNullOrEmpty(searchOptions.CategoryName))
            {
                query = query.Where(c => c.Category.CategoryName.Contains(searchOptions.CategoryName));
            }

            if (!string.IsNullOrEmpty(searchOptions.Username))
            {
                query = query.Where(c => c.User.UserName.Contains(searchOptions.Username));
            }


            var courses = await query.ToListAsync();

            var courseViews = _map.Map<IEnumerable<CourseViewGET>>(courses);

            return courseViews.OrderByDescending(c => c.AverageStarRating)
                              .ThenByDescending(c => c.CourseTitle);
        }

        public async Task<Course> GetByIdAsync(int id)
        {
            return await Entities.FirstOrDefaultAsync(x => x.CourseId == id && x.Status.ToLower() == "active");
        }

        public async Task<Course> GetByIdAllStatusAsync(int id)
        {
            return await Entities.FirstOrDefaultAsync(x => x.CourseId == id);
        }

        public async Task<Course> GetInactiveByIdAsync(int id)
        {
            return await Entities.FirstOrDefaultAsync(x => x.CourseId == id && x.Status.ToLower() == "inactive");
        }

        public async Task<Course> UpdateAsync(int id, UpdateCourseModel course)
        {

            var existingCourse = await GetByIdAsync(id);

            if (existingCourse == null)
            {
                throw new KeyNotFoundException($"Course with Id {id} not found.");
            }


            var courseSimilarValidation = await Entities.FirstOrDefaultAsync(c => c.CourseTitle == course.CourseTitle && c.CourseId != id);

            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }

            if (courseSimilarValidation != null)
            {
                throw new InvalidOperationException($"Course with title '{course.CourseTitle}' already exists.");
            }

            var categoryExists = await _dbContext.Category.AnyAsync(c => c.CategoryId == course.CategoryId && c.Status.ToLower() == "active");
            if (!categoryExists)
            {
                throw new KeyNotFoundException($"Category with Id {course.CategoryId} not found.");
            }

            if (course.Image != null)
            {
                var imageUrl = await UploadFileAsync(course.Image);
                existingCourse.Image = imageUrl;
            }

            existingCourse.CategoryId = course.CategoryId ?? existingCourse.CategoryId;
            existingCourse.CourseTitle = course.CourseTitle ?? existingCourse.CourseTitle;
            existingCourse.Description = course.Description ?? existingCourse.Description;
            existingCourse.Discount = course.Discount ?? 0;
            existingCourse.Level = course.Level ?? existingCourse.CategoryId;
            existingCourse.CreatedDate = course.CreatedDate;
            existingCourse.IsComment = course.IsComment ?? false;
            existingCourse.Price = course.Price ?? existingCourse.Price;
            existingCourse.ShortDescription = course.ShortDescription ?? existingCourse.ShortDescription;
            //_dbContext.Course.Update(course);
            Entities.Update(existingCourse);
            await _dbContext.SaveChangesAsync();
            return existingCourse;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                var stream = file.OpenReadStream();
                var bucket = configuration["FireBase:Bucket"];

                var task = new FirebaseStorage(bucket)
                    .Child("Image_Course")
                    .Child(file.FileName)
                    .PutAsync(stream);

                var downloadUrl = await task;

                return downloadUrl;
            }
            return null;
        }

        // Adjusted while Migrating DB (Feedback N:1 PurchasedCourse)
        public async Task<double> GetCourseRatingAsync(int courseId)
        {
            var course = await Entities.Include(c => c.PurchasedCourses)
                                       .ThenInclude(pc => pc.Feedback)
                                       .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return 0;
            }

            var feedbacks = course.PurchasedCourses.SelectMany(pc => pc.Feedback).ToList();
            if (!feedbacks.Any())
            {
                return 0;
            }

            double totalStar = feedbacks.Where(f => f.Star.HasValue).Sum(f => f.Star.Value);
            double rate = totalStar / feedbacks.Count(f => f.Star.HasValue);

            return rate;
        }

        public async Task<Course> AddCourseAsync(CreateCourseModel createCourseModel, string userName)
        {
            if (createCourseModel == null)
            {
                throw new ArgumentNullException(nameof(createCourseModel));
            }

            var categoryExists = await _dbContext.Category.AnyAsync(c => c.CategoryId == createCourseModel.CategoryId && c.Status.ToLower() == "active");

            if (!categoryExists)
            {
                throw new InvalidOperationException("Category does not exist.");
            }

            var existingCourse = await Entities.FirstOrDefaultAsync(c => c.CourseTitle == createCourseModel.CourseTitle);

            if (existingCourse != null)
            {
                throw new InvalidOperationException("Course title must be unique.");
            }

            var lastCourse = await _dbContext.Course
                .Where(c => c.CourseCode.StartsWith("CO"))
                .OrderByDescending(c => c.CourseCode)
                .FirstOrDefaultAsync();

            int nextCourseNumber = 1;

            if (lastCourse != null)
            {
                string lastCourseCode = lastCourse.CourseCode.Substring(2);

                if (int.TryParse(lastCourseCode, out int currentCourseNumber))
                {
                    nextCourseNumber = currentCourseNumber + 1;
                }
            }

            string nextCourseCode = $"CO{nextCourseNumber:D4}";

            var course = new Course
            {
                CategoryId = createCourseModel.CategoryId,
                CourseTitle = createCourseModel.CourseTitle,
                CourseCode = nextCourseCode,
                Description = createCourseModel.Description,
                Discount = createCourseModel.Discount ?? 0,
                Level = createCourseModel.Level,
                IsComment = createCourseModel.IsComment,
                TotalEnrollment = createCourseModel.TotalEnrollment,
                Status = createCourseModel.Status,
                Price = createCourseModel.Price,
                ShortDescription = createCourseModel.ShortDescription ?? null,
                Username = userName,
                CreatedDate = createCourseModel.CreatedDate ?? DateTime.UtcNow.AddHours(7),
            };

            if (createCourseModel.Image != null)
            {
                var imageUrl = await UploadFileAsync(createCourseModel.Image);
                course.Image = imageUrl;
            }

            await _dbContext.Course.AddAsync(course);
            await _dbContext.SaveChangesAsync();

            return course;
        }




        public async Task<string> UploadFileAsyncc(IFormFile file)
        {
            if (file.Length > 0)
            {
                var stream = file.OpenReadStream();
                var bucket = configuration["FireBase:Bucket"];

                var task = new FirebaseStorage(bucket)
                    .Child("Image_Course")
                    .Child(file.FileName)
                    .PutAsync(stream);

                var downloadUrl = await task;
                return downloadUrl;
            }
            return null;
        }
        public async Task<Course> ConfirmCourse(int courseID, bool accept)
        {
            var course = await Entities.FirstOrDefaultAsync(c => c.CourseId == courseID);

            if (course == null)
            {
                throw new KeyNotFoundException("Course does not exist");
            }

            course.Status = accept ? "Active" : "Inactive";

            await _dbContext.SaveChangesAsync();

            return course;
        }
        public async Task<List<Course>> GetByIdAsync(List<int> ids)
        {
            return await Entities
                .Where(x => ids.Contains(x.CourseId) && x.Status.ToLower() == "active")
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<Course> ResubmitCourse(int courseId, ResubmitCourseModel reasonContent)
        {
            var course = await GetInactiveByIdAsync(courseId);
            if (course == null)
            {
                throw new KeyNotFoundException($"Course with Id {courseId} not found.");
            }

            course.Status = "Pending";

            var reason = new Reason
            {
                Content = reasonContent.reasonContent,
                Status = "Pending",
                CourseId = course.CourseId
            };

            //await reasonRepository.AddAsync(reason);

            _dbContext.Reason.Add(reason);
            await _dbContext.SaveChangesAsync();

            //return reason;

            return course;
        }

        public async Task<Course> GetByIdOrCodeAsync(int? id = null, string? Code = null)
        {
            if (id.HasValue)
            {
                return await _dbContext.Course.FirstOrDefaultAsync(c => c.CourseId == id.Value);
            }
            else if (!string.IsNullOrEmpty(Code))
            {
                return await _dbContext.Course.FirstOrDefaultAsync(c => c.CourseCode == Code);
            }

            return null;
        }

        public async Task<IEnumerable<CourseViewRevenue>> GetViewRevenuesAsync(string userName)
        {
            var courses = await Entities
                            .Include(c => c.Category)
                            .Include(u => u.User).Where(u => u.Username == userName)
                            .Include(od => od.OrderDetails)
                            .ThenInclude(ord => ord.Order)
                            .Where(e => e.OrderDetails.Any(od => od.Order.Status.ToLower() == "success"))
                            .ToListAsync();

            if (courses == null || !courses.Any())
            {
                return null;
            }

            var courseView =  _map.Map<IEnumerable<CourseViewRevenue>>(courses);

            return courseView;
        }
    }
}
