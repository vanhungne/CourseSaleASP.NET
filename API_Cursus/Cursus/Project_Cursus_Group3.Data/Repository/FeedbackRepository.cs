using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Extensions;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.FeedBackModel;
using Project_Cursus_Group3.Data.ViewModels.FeedbackDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Repository
{
    public class FeedbackRepository : Repository<Feedback> , IFeedbackRepository
    {
       private readonly CursusDbContext _context;
       private readonly IConfiguration _configuration;

        public FeedbackRepository(CursusDbContext context, IConfiguration configuration) : base(context)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Feedback?> GetFeedbackByIdAsync(int id)
        {
            return await Entities
                .Include(x => x.PurchasedCourse)
                    .ThenInclude(pc => pc.Course)
                .FirstOrDefaultAsync(x => x.FeedbackId == id);
        }


        public async Task UpdateFeedback(Feedback feedback)
        {
            _context.Feedback.Update(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FeedbackViewModel>> ViewAcceptedFeedbacksAsync(
            string userName,
            string? searchContent = "",
            string? sortBy = "createdate",
            bool ascending = true,
            int? filterCourseID = null)
        {

            var userHasFeedback = await _context.Feedback.Include(x => x.PurchasedCourse)
                    .ThenInclude(pc => pc.Course)
        .AnyAsync(f => f.UserName == userName);

            if (!userHasFeedback)
            {
                throw new InvalidOperationException("This user has no feedback.");
            }

            var feedbacksQuery = _context.Feedback
                .FilterByStatus("Accept")
                .FilterByUserName(userName)
                .SearchContent(searchContent)
                .Sort(sortBy, ascending)
                .FilterByCourseId(filterCourseID);

            var feedbacks = await feedbacksQuery
                .Select(f => new FeedbackViewModel
                {
                    CourseId = f.CourseId,
                    UserName = f.UserName,
                    Content = f.Content,
                    Attachment = f.Attachment,
                    Star = f.Star,
                    CreatedDate = f.CreatedDate,
                    Status = f.Status
                })
                .ToListAsync();

            return feedbacks;
        }

        public async Task<AddFeedbackModel> AddFeedbackAsync(AddFeedbackModel addFeedbackModel, IFormFile? attachmentFile,string userName)
        {
            if (addFeedbackModel == null)
            {
                throw new ArgumentNullException(nameof(addFeedbackModel));
            }

            var course = await _context.Course
        .FirstOrDefaultAsync(c => c.CourseId == addFeedbackModel.CourseId);

            if (course == null || !course.IsComment)
            {
                throw new InvalidOperationException("Feedback is not allowed for this course.");
            }

            var purchased = await _context.PurchasedCourse
         .AnyAsync(pc => pc.CourseId == addFeedbackModel.CourseId && pc.UserName == userName);

            if (!purchased)
            {
                throw new InvalidOperationException($"User {userName} must purchase the course to provide feedback.");
            }

            string attachmentUrl = null;

            if (attachmentFile != null && attachmentFile.Length > 0)
            {
                attachmentUrl = await UploadFileAsync(attachmentFile);
            }

            var feedback = new Feedback
            {
                CourseId = addFeedbackModel.CourseId,
                UserName = userName,
                Content = addFeedbackModel.Content,
                Attachment = attachmentUrl,
                Star = addFeedbackModel.Star,
                CreatedDate = DateTime.Now,
                Status = "Accept"
            };

            _context.Feedback.Add(feedback);
            await _context.SaveChangesAsync();

            return addFeedbackModel;
        }

        public async Task UpdateFeedbackAsync(int id, UpdateFeedbackModel updateFeedbackModel, IFormFile? attachmentFile)
        {
            if (updateFeedbackModel == null)
            {
                throw new ArgumentNullException(nameof(updateFeedbackModel));
            }

            var existingFeedback = await GetFeedbackByIdAsync(id);
            if (existingFeedback == null)
            {
                throw new KeyNotFoundException($"Feedback with Id {id} not found.");
            }

            existingFeedback.Star = updateFeedbackModel.Star ?? existingFeedback.Star;

            existingFeedback.Content = updateFeedbackModel.Content ?? existingFeedback.Content;


            if (attachmentFile != null && attachmentFile.Length > 0)
            {
                var attachmentUrl = await UploadFileAsync(attachmentFile);
                existingFeedback.Attachment = attachmentUrl;
            }

            existingFeedback.CreatedDate = DateTime.UtcNow.AddHours(7);

            await UpdateFeedback(existingFeedback);
        }

        public async Task<Feedback> DeleteFeedbackAsync(int id)
        {
            var feedback = await GetFeedbackByIdAsync(id);
            if (feedback == null)
            {
                throw new KeyNotFoundException($"Feedback with Id {id} not found.");
            }

            if (feedback.Status.ToLower() == "reject")
            {
                throw new InvalidOperationException("The feedback is already rejected.");
            }

            feedback.Status = "Reject";
            await _context.SaveChangesAsync();

            return feedback;
        }

        public async Task<string?> ReadAttachmentAsync(IFormFile attachmentFile)
        {
            using (var reader = new StreamReader(attachmentFile.OpenReadStream()))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                var stream = file.OpenReadStream();
                var bucket = _configuration["FireBase:Bucket"];

                var task = new FirebaseStorage(bucket)
                    .Child("Feedback")
                    .Child(file.FileName)
                    .PutAsync(stream);

                var downloadUrl = await task;

                return downloadUrl;
            }
            return null;
        }

    }
}
