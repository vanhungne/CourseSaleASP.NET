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
    public class PurchasedCourseRepository : Repository<PurchasedCourse>, IPurchasedCourseRepository
    {
        private readonly CursusDbContext _dbcontext;
        public PurchasedCourseRepository(CursusDbContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task AddPurchaseCourse(PurchasedCourse purchaseCourse)
        {
           await _dbcontext.PurchasedCourse.AddAsync(purchaseCourse);
            await _dbcontext.SaveChangesAsync();

        }

        public async Task<List<int>> GetEnrolledCourseIdsByUserNameAsync(string userName)
        {
            var courseIds = await Entities
                                      .Where(pc => pc.UserName == userName && pc.Status.ToLower() == "Active")
                                      .Include(x => x.User)
                                      .Select(pc => pc.CourseId)
                                      .ToListAsync();
            return courseIds;
        }
        

    }
}
