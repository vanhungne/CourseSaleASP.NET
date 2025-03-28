using Microsoft.AspNetCore.Http;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.FeedBackModel;
using Project_Cursus_Group3.Data.Repository;
using Project_Cursus_Group3.Data.ViewModels.FeedbackDTO;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service
{
    public class FeedbackServices : IFeedbackServices
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackServices(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<Feedback?> GetFeedbackByIdAsync(int id)
        {
            return await _feedbackRepository.GetFeedbackByIdAsync(id);
        }

        public async Task<List<FeedbackViewModel>> ViewAcceptedFeedbacksAsync(string userName, string? searchContent, string? sortBy, bool ascending, int? filterCourseID)
        {
            return await _feedbackRepository.ViewAcceptedFeedbacksAsync(userName, searchContent, sortBy, ascending, filterCourseID);
        }

        public async Task<AddFeedbackModel> AddFeedbackAsync(AddFeedbackModel addFeedbackModel, IFormFile attachmentFile, string userName)
        {
            if (addFeedbackModel == null)
            {
                throw new ArgumentNullException(nameof(addFeedbackModel));
            }

            await _feedbackRepository.AddFeedbackAsync(addFeedbackModel, attachmentFile, userName);
            return addFeedbackModel;
        }

        public Task UpdateFeedback(Feedback feedback)
        {
            return _feedbackRepository.UpdateFeedback(feedback);
        }

        public async Task UpdateFeedbackAsync(int id, UpdateFeedbackModel updateFeedbackModel, IFormFile? attachmentFile)
        {
            if (updateFeedbackModel == null)
            {
                throw new ArgumentNullException(nameof(updateFeedbackModel));
            }

            await _feedbackRepository.UpdateFeedbackAsync(id, updateFeedbackModel, attachmentFile);
        }

        public async Task<Feedback> DeleteFeedbackAsync(int id)
        {
            return await _feedbackRepository.DeleteFeedbackAsync(id);
        }
    }
}
