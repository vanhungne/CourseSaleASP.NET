using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.LessonModel;
using Project_Cursus_Group3.Data.ViewModels.Filter;
using Project_Cursus_Group3.Data.ViewModels.LessonDTO;
using System.Security.Claims;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface ILessonServices
    {
        Task<List<LessonViewModel>> ViewActiveLessonsAsync(
string userName,
    List<FilterCriteria> filters,
    string? sortBy,
    bool isAscending,
    int pageNumber,
    int pageSize,
    int? filterDuration);
        Task<AddLessonModel> AddLessonAsync(AddLessonModel addLessonModel, string userName);
        Task UpdateLesson(Lesson chapter);
        Task UpdateLessonAsync(int id, UpdateLessonModel updateLessonModel);
        Task<Lesson> DeleteLessonAsync(int id);
        Task<Lesson> GetLessonByIdAsync(int id, ClaimsPrincipal userClaims);
    }
}
