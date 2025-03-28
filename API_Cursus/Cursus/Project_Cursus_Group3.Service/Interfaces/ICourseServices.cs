using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.ViewModels.CourseDTO;
using Project_Cursus_Group3.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Project_Cursus_Group3.Data.Model.CourseModel;
using Project_Cursus_Group3.Data.Model;
using Project_Cursus_Group3.Data.Model.ReasonModel;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface ICourseServices
    {
        Task<Course> GetByIdOrCodeAsync(int? id = null, string? Code = null);
        Task<Course> GetByIdAllStatusAsync(int id);
        Task<Course> GetByIdAsync(int id);
        Task<Course> GetInactiveByIdAsync(int id);
        Task<IEnumerable<Course>> GetAllAsync();
        Task<Course> AddAsync(CreateCourseModel createCourseModel, string userName);

        Task<Course> UpdateAsync(int id, UpdateCourseModel course);
        Task<Course> DeleteAsync(int id, string reasonContent);
        Task<IEnumerable<CourseViewGET>> GetAllCoursesActive(CourseSearchOptions search);

        Task<Course> ConfirmCourse(int id, bool accept);

        Task<List<Course>> GetByIdAsync(List<int> ids);

        Task<Course> ResubmitCourse(int courseId, ResubmitCourseModel reasonContent);

        Task<IEnumerable<CourseViewRevenue>> GetViewRevenuesAsync(string userName);
    }
}
