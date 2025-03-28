using Microsoft.AspNetCore.Http;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model;
using Project_Cursus_Group3.Data.Model.CourseModel;
using Project_Cursus_Group3.Data.Model.ReasonModel;
using Project_Cursus_Group3.Data.Repository;
using Project_Cursus_Group3.Data.ViewModels.CourseDTO;
using Project_Cursus_Group3.Service.Interfaces;

namespace Project_Cursus_Group3.Service.Repository
{
    public class CourseServices : ICourseServices
    {
        private readonly ICourseRepository courseRepository;

        public CourseServices(ICourseRepository courseRepository)
        {
            this.courseRepository = courseRepository;
        }

        public Task<Course> AddAsync(Course course)
        {
            throw new NotImplementedException();
        }

        public Task<Course> DeleteAsync(int id, string reasonContent)
        {
            return courseRepository.DeleteAsync(id, reasonContent);
        }
        public Task<Course> GetByIdAsync(int id)
        {
            return courseRepository.GetByIdAsync(id);
        }

        public Task<Course> GetByIdAllStatusAsync(int id)
        {
            return courseRepository.GetByIdAllStatusAsync(id);
        }

        public Task<Course> UpdateAsync(int id, UpdateCourseModel course)
        {
            return courseRepository.UpdateAsync(id, course);
        }

        public Task<IEnumerable<CourseViewGET>> GetAllCoursesActive(CourseSearchOptions search)
        {
            return courseRepository.GetAllCourseActiveAsync(search);
        }

        public Task<IEnumerable<Course>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Course> AddAsync(CreateCourseModel createCourseModel, string userName)
        {
            return await courseRepository.AddCourseAsync(createCourseModel, userName);
        }

        public async Task<Course> ConfirmCourse(int id, bool accept)
        {
            return await courseRepository.ConfirmCourse(id, accept);

        }
        public async Task<List<Course>> GetByIdAsync(List<int> ids)
        {
            return await courseRepository.GetByIdAsync(ids);
        }
        public async Task<Course> GetInactiveByIdAsync(int id)
        {
            return await courseRepository.GetInactiveByIdAsync(id);
        }

        public async Task<Course> ResubmitCourse(int courseId, ResubmitCourseModel reasonContent)
        {
            return await courseRepository.ResubmitCourse(courseId, reasonContent);
        }

        public async Task<Course> GetByIdOrCodeAsync(int? id = null, string? Code = null)
        {
            return await courseRepository.GetByIdOrCodeAsync(id, Code);
        }

        public async Task<IEnumerable<CourseViewRevenue>> GetViewRevenuesAsync(string userName)
        {
            return await courseRepository.GetViewRevenuesAsync(userName);
        }
    }
}
