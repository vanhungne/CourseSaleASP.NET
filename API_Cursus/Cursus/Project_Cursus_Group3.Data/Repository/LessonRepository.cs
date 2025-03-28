using AutoMapper;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.LessonModel;
using Project_Cursus_Group3.Data.ViewModels.Filter;
using Project_Cursus_Group3.Data.ViewModels.LessonDTO;
using System.Security.Claims;

namespace Project_Cursus_Group3.Data.Repository
{
    public class LessonRepository : Repository<Lesson>, ILessonRepository
    {
        private readonly CursusDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly UploadFile _upload;

        public LessonRepository(CursusDbContext context, IConfiguration configuration, IMapper mapper, UploadFile upload) : base(context)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _upload = upload;
        }

        public async Task<Lesson> GetLessonByIdAsync(int id, ClaimsPrincipal userClaims)
        {
            var username = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return await Entities.Include(x => x.Chapter).FirstOrDefaultAsync(x => x.LessonId == id && x.Status.ToLower() == "Active" && x.Chapter.Course.Username == username);
        }

        public async Task<Lesson> AddLessonAsync(AddLessonModel addLessonModel, string userName)
        {
            var chapter = await _context.Chapter.Include(x => x.Course).FirstOrDefaultAsync(c => c.ChapterId == addLessonModel.ChapterId);
            var lesson = _mapper.Map<Lesson>(addLessonModel);

            if (chapter == null)
            {
                throw new KeyNotFoundException("Chapter does not exist.");
            }

            if (chapter.Course.Username != userName)
            {
                throw new InvalidOperationException("User cannot add lesson to a chapter that does not belong to them.");
            }

            if (await Entities.AnyAsync(c => c.ChapterId == addLessonModel.ChapterId && c.LessonTitle == addLessonModel.LessonTitle))
            {
                throw new InvalidOperationException("Lesson title must be unique within the chapter.");
            }

            if (addLessonModel.VideoURL != null)
            {
                var lessonVideo = await UploadFileAsync(addLessonModel.VideoURL);
                lesson.VideoURL = lessonVideo;
            }

            Entities.Add(lesson);
            await _context.SaveChangesAsync();

            return lesson;
        }


        public async Task<Lesson> DeleteLessonAsync(int id)
        {
            var lesson = await Entities
                .Include(c => c.Chapter)
                .FirstOrDefaultAsync(c => c.LessonId == id);

            if (lesson == null)
            {
                throw new KeyNotFoundException($"Lesson {id} does not exist.");
            }

            lesson.Status = "Inactive";
            await _context.SaveChangesAsync();

            return lesson;
        }


        public async Task UpdateLessonAsync(int id, UpdateLessonModel updateLessonModel)
        {
            var lesson = await Entities.FindAsync(id);
            _mapper.Map(updateLessonModel, lesson);

            if (lesson == null)
            {
                throw new KeyNotFoundException("Lesson does not exist.");
            }

            if (updateLessonModel.ChapterId.HasValue)
            {
                var chapterExists = await _context.Chapter.AnyAsync(c => c.ChapterId == updateLessonModel.ChapterId.Value);
                if (!chapterExists)
                {
                    throw new InvalidOperationException($"Chapter {updateLessonModel.ChapterId} does not exist.");
                }
                lesson.ChapterId = updateLessonModel.ChapterId;
            }

            if (await Entities.AnyAsync(c => c.ChapterId == lesson.ChapterId &&
                                             c.LessonTitle == lesson.LessonTitle &&
                                             c.LessonId != lesson.LessonId))
            {
                throw new InvalidOperationException("Lesson title must be unique within the chapter.");
            }
            if (updateLessonModel.VideoURL != null)
            {
                var lessonVideo = await UploadFileAsync(updateLessonModel.VideoURL);
                lesson.VideoURL = lessonVideo;
            }
            Entities.Update(lesson);
            await _context.SaveChangesAsync();

        }

    public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                var stream = file.OpenReadStream();
                var bucket = _configuration["FireBase:Bucket"];

                var task = new FirebaseStorage(bucket)
                    .Child("Video")
                    .Child(file.FileName)
                    .PutAsync(stream);

                var downloadUrl = await task;

                return downloadUrl;
            }
            return null;
        }

        public async Task<List<LessonViewModel>> ViewActiveLessonsAsync(
        string userName,
        List<FilterCriteria>? filters = null,
        string? sortBy = null,
        bool isAscending = true,
        int pageNumber = 1,
        int pageSize = 10,
        int? filterDuration = null)
        {
            // Start with a base query
            var lessonsQuery = Entities
                .Include(x => x.Chapter)
                .Where(c => c.Chapter.Course.Username == userName && c.Status == "Active")
                .AsQueryable();

            // Apply additional filtering if filters are provided
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    if (!string.IsNullOrEmpty(filter.FilterOn) && !string.IsNullOrEmpty(filter.FilterQuery))
                    {
                        if (filter.FilterOn.Equals("LessonTitle", StringComparison.OrdinalIgnoreCase))
                        {
                            lessonsQuery = lessonsQuery.Where(x => x.LessonTitle.Contains(filter.FilterQuery));
                        }
                        //else if (filter.FilterOn.Equals("ChapterTitle", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    lessonsQuery = lessonsQuery.Where(x => x.Chapter.ChapterTitle.Contains(filter.FilterQuery));
                        //}

                    }
                }
            }

            // Filter by lesson duration if specified
            if (filterDuration.HasValue)
            {
                lessonsQuery = lessonsQuery.Where(x => x.Duration == filterDuration.Value);
            }

            // Sorting
            if (sortBy != null)
            {
                if (sortBy.Equals("LessonTitle", StringComparison.OrdinalIgnoreCase))
                {
                    lessonsQuery = isAscending ? lessonsQuery.OrderBy(x => x.LessonTitle) : lessonsQuery.OrderByDescending(x => x.LessonTitle);
                }
                else if (sortBy.Equals("ChapterTitle", StringComparison.OrdinalIgnoreCase))
                {
                    lessonsQuery = isAscending ? lessonsQuery.OrderBy(x => x.Chapter.ChapterTitle) : lessonsQuery.OrderByDescending(x => x.Chapter.ChapterTitle);
                }
            }

            // Paging
            var skip = (pageNumber - 1) * pageSize;
            var lessons = _mapper.Map<List<LessonViewModel>>(lessonsQuery);
            return lessons.Skip(skip).Take(pageSize).ToList();

        }


    }
}
