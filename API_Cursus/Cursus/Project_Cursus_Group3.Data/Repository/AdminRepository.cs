using Microsoft.EntityFrameworkCore;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly CursusDbContext _dbContext;

        public AdminRepository(CursusDbContext context)
        {
            _dbContext = context;
        }
        public async Task<User?> GetByIdAsync(string userName)
        {
            return await _dbContext.User.FindAsync(userName);
        }

        public async Task UpdateAsync(User user)
        {
            _dbContext.User.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ToggleUserStatusAsync(string userName, bool isActive, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new ArgumentException("A comment is required to change the status.");
            }

            var user = await GetByIdAsync(userName);
            if (user == null)
            {
                throw new KeyNotFoundException($"User {userName} not found.");
            }

            if ((user.Status.ToLower() == "active" && isActive) || (user.Status.ToLower() == "inactive" && !isActive))
            {
                throw new InvalidOperationException($"The Status is already {(isActive ? "active" : "inactive")}.");
            }

            user.Status = isActive ? "active" : "inactive";
            user.deleteComment = comment;
            await UpdateAsync(user);

        }

        public async Task ConfirmReasonAsync(int courseId, bool isApproved)
        {
            var reason = await _dbContext.Reason.FirstOrDefaultAsync(x => x.CourseId == courseId);

          
            if (reason == null)
            {
                throw new KeyNotFoundException($"Reason for Course ID {courseId} does not exist.");
            }


            var course = await _dbContext.Course
                .FirstOrDefaultAsync(x => x.CourseId == courseId && x.Status.ToLower() == "pending".ToLower());


            if (course == null)
            {
                throw new KeyNotFoundException($"Course with ID {courseId} not found or not pending.");
            }

         
            course.Status = isApproved ? "inactive" : "active";
            reason.Status = isApproved ? "accept" : "reject";

            
            _dbContext.Course.Update(course);
            _dbContext.Reason.Update(reason);

            
            await _dbContext.SaveChangesAsync();
        }



    }

}
