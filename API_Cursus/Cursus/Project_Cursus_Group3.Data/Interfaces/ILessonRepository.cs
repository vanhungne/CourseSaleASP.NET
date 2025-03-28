using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.LessonModel;
using Project_Cursus_Group3.Data.ViewModels.Filter;
using Project_Cursus_Group3.Data.ViewModels.LessonDTO;
using System.Security.Claims;

namespace Project_Cursus_Group3.Data.Interfaces
{
    public interface ILessonRepository
    {
        Task<Lesson> GetLessonByIdAsync(int id, ClaimsPrincipal userClaims);
        Task<List<LessonViewModel>> ViewActiveLessonsAsync(
                   string userName,
    List<FilterCriteria> filters,
    string? sortBy,
    bool isAscending,
    int pageNumber,
    int pageSize,
    int? filterDuration);
        Task<Lesson> AddLessonAsync(AddLessonModel addLessonModel, string userName);
        Task UpdateLessonAsync(int id, UpdateLessonModel updateLessonModel);
        Task<Lesson> DeleteLessonAsync(int id);

    }
}
