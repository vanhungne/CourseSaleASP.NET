using Microsoft.AspNetCore.Http;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.FeedBackModel;
using Project_Cursus_Group3.Data.ViewModels.FeedbackDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<Feedback?> GetFeedbackByIdAsync(int id);
        Task<List<FeedbackViewModel>> ViewAcceptedFeedbacksAsync(string userName, string? searchContent, string? sortBy, bool ascending, int? filterCourseID);
        Task<AddFeedbackModel> AddFeedbackAsync(AddFeedbackModel addFeedbackModel, IFormFile attachmentFile, string userName);
        Task UpdateFeedback(Feedback feedback);
        Task<string?> ReadAttachmentAsync(IFormFile attachmentFile);
        Task UpdateFeedbackAsync(int id, UpdateFeedbackModel updateFeedbackModel, IFormFile? attachmentFile);
        Task<Feedback> DeleteFeedbackAsync(int id);
    }
}
