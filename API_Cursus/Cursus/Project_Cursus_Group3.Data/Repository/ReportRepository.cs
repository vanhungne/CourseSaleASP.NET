using AutoMapper;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model;
using Project_Cursus_Group3.Data.Model.ReportModel;
using Project_Cursus_Group3.Data.ViewModels.Report;
using System;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Repository
{
    public class ReportRepository : Repository<Report>, IReportRepository
    {
        private readonly IConfiguration _configuration;
        private readonly CursusDbContext _dbContext;
        private readonly IMapper _mapper;

        public ReportRepository(IConfiguration configuration, CursusDbContext dbContext, IMapper mapper) : base(dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Report> AddReportAsync(CreateReportModel model, IFormFile file)
        {

            var purchasedCourse = await _dbContext.PurchasedCourse.Include(x => x.Course).Include(x => x.Feedback)
                .FirstOrDefaultAsync(pc => pc.CourseId == model.CourseId && pc.UserName == model.UserName);

            if (purchasedCourse == null)
            {
                throw new InvalidOperationException("No purchased course found for the specified CourseId and UserName.");
            }

            // Check if a report already exists for this purchased course
            var existingReport = await Entities
                .FirstOrDefaultAsync(r => r.CourseId == model.CourseId && r.UserName == model.UserName);

            if (existingReport != null)
            {
                throw new InvalidOperationException("A report already exists for the specified CourseId and UserName.");
            }

            string attachmentUrl = file != null ? await UploadFileAsync(file) : null;

            var report = _mapper.Map<Report>(model);
            report.Attachment = attachmentUrl;
            report.Status = "Accept";
            report.UserName = purchasedCourse.UserName;

            await _dbContext.Report.AddAsync(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<Report> GetByIdSync(int id)
        {
            return await Entities
              .Include(r => r.PurchasedCourse)
              .ThenInclude(pc => pc.Course)
              .FirstOrDefaultAsync(r => r.ReportId == id);
        }

        public async Task<Report> UpdateReportAsync(int id, Report updatedReport, IFormFile file = null)
        {
            var existingReport = await GetByIdSync(id);
            if (existingReport == null)
            {
                throw new KeyNotFoundException($"Report with Id {id} not found.");
            }
            if (!string.IsNullOrWhiteSpace(updatedReport.Issue))
            {
                existingReport.Issue = updatedReport.Issue;
            }
            if (!string.IsNullOrWhiteSpace(updatedReport.Content))
            {
                existingReport.Content = updatedReport.Content;
            }
            if (file != null && file.Length > 0)
            {
                string attachmentUrl = await UploadFileAsync(file);
                if (!string.IsNullOrEmpty(attachmentUrl))
                {
                    existingReport.Attachment = attachmentUrl;
                }
            }

            await _dbContext.SaveChangesAsync();
            return await GetByIdSync(id);
        }


        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var bucket = _configuration["FireBase:Bucket"];
            using var stream = file.OpenReadStream();
            var task = new FirebaseStorage(bucket)
                .Child("report")
                .Child(file.FileName)
                .PutAsync(stream);

            return await task;
        }

        public async Task<IEnumerable<Report>> GetReportsByUserNameAsync(string userName)
        {
            return await Entities
                             .Where(r => r.UserName == userName && r.Status.ToLower() == "Accept")
                             .Include(r => r.PurchasedCourse)
                             .ThenInclude(pc => pc.Course)
                             .ToListAsync();
        }

        public async Task<IEnumerable<Report>> SearchReportsAsync(
   string userName,
   string? contentSearch,
   string? courseTitleSearch,
   string? issueSearch)
        {
            var query = Entities
                .Where(r => r.UserName == userName && r.Status.ToLower() == "Accept")
                .Include(r => r.PurchasedCourse)
                .ThenInclude(pc => pc.Course)
                .AsQueryable();

            // Apply each filter conditionally
            if (!string.IsNullOrEmpty(contentSearch))
            {
                query = query.Where(r => r.Content.Contains(contentSearch));
            }

            if (!string.IsNullOrEmpty(courseTitleSearch))
            {
                query = query.Where(r => r.PurchasedCourse.Course.CourseTitle.Contains(courseTitleSearch));
            }

            if (!string.IsNullOrEmpty(issueSearch))
            {
                query = query.Where(r => r.Issue.Contains(issueSearch));
            }

            return await query.ToListAsync();
        }

       

        public async Task<Report> UpdateReportStatusAsync(int reportId, string newStatus)
        {
            var report = await Entities.FirstOrDefaultAsync(r => r.ReportId == reportId);

            if (report == null)
            {
                return null;
            }

            report.Status = newStatus;

            Entities.Update(report);
            await _dbContext.SaveChangesAsync();

            return report;
        }

    }
}