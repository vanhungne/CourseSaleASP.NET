using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.ReasonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Repository
{
    public class ReasonRepository : Repository<Reason>, IReasonRepository
    {
        private readonly IMapper _map;
        private readonly CursusDbContext _dbContext;
        private readonly IConfiguration configuration;

        public ReasonRepository(CursusDbContext context, IMapper mapper, IConfiguration configuration) : base(context)
        { 
            _dbContext = context;
            this.configuration = configuration;
            _map = mapper;
        }

        /*public async Task<Reason?> GetByIdAsync(int id)
        {
            return await Entities.Include(x => x.Course).FirstOrDefaultAsync(x => x.ReasonId == id && x.Status == "Accept");
        }*/

        public async Task<Reason> AddAsync(Reason reason)
        {
            if(reason == null)
            {
                throw new ArgumentNullException(nameof(reason));
        }
            _dbContext.Reason.Add(reason);
            await _dbContext.SaveChangesAsync();
            return reason;
        }
        
        public async Task<Reason?> DeleteAsync(int id)
        {
            var report = await GetByIdAsync(id);

            if (report == null)
            {
                throw new Exception($"Reason with ID {id} not found.");
            }

            report.Status = "Reject";

            Entities.Update(report);
            await _dbContext.SaveChangesAsync();

            return report;

        }

        public async Task<Reason> UpdateAsync(int id, UpdateReasonModel reason)
        {
            var existingReason = await GetByIdAsync(id);

            if (existingReason == null)
            { 
                throw new KeyNotFoundException($"Category with Id {id} not found.");
            }
;
            existingReason.Content = reason.Content;

            Entities.Update(existingReason);
            await _dbContext.SaveChangesAsync();

            return existingReason;
            
        }

        public async Task<Reason> GetByIdAsync(int id)
        {
            return await Entities.FirstOrDefaultAsync(r => r.ReasonId == id && r.Status.ToLower() == "pending");
        }
        public async Task<List<Reason>> GetReasonsByUsernameAsync(string username, string reasonContent = null, string courseTitle = null)
        {
            var query = _dbContext.Reason
                .Where(r => r.Course.Username == username)
                .AsQueryable();

            if (!string.IsNullOrEmpty(reasonContent))
            {
                query = query.Where(r => r.Content.Contains(reasonContent));
            }

            if (!string.IsNullOrEmpty(courseTitle))
            {
                query = query.Where(r => r.Course.CourseTitle.Contains(courseTitle));
            }

            var results = await query.ToListAsync();


            return results;
        }
    }
}
