using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.LessonModel;
using Project_Cursus_Group3.Data.ViewModels.Filter;
using Project_Cursus_Group3.Data.ViewModels.LessonDTO;
using Project_Cursus_Group3.Service.Interfaces;
using System.Security.Claims;

namespace Project_Cursus_Group3.Service.Services
{

    public class LessonServices : ILessonServices
    {
        private readonly ILessonRepository _lessonRepository;

        public LessonServices(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        public async Task<AddLessonModel> AddLessonAsync(AddLessonModel addLessonModel, string userName)
        {
            await _lessonRepository.AddLessonAsync(addLessonModel, userName);
            return addLessonModel;
        }

        public async Task<Lesson> DeleteLessonAsync(int id)
        {
            return await _lessonRepository.DeleteLessonAsync(id);
        }


        public async Task<Lesson?> GetLessonByIdAsync(int id, ClaimsPrincipal userClaims)
        {
            return await _lessonRepository.GetLessonByIdAsync(id, userClaims);
        }

        public Task UpdateLesson(Lesson lesson)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateLessonAsync(int id, UpdateLessonModel updateLessonModel)
        {
            await _lessonRepository.UpdateLessonAsync(id, updateLessonModel);
        }

        public async Task<List<LessonViewModel>> ViewActiveLessonsAsync(string userName,
    List<FilterCriteria> filters,
    string? sortBy,
    bool isAscending,
    int pageNumber,
    int pageSize,
    int? filterDuration)
        {
            return await _lessonRepository.ViewActiveLessonsAsync(userName, filters, sortBy, isAscending, pageNumber, pageSize, filterDuration);
        }
    }
}

