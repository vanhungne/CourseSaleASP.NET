using Microsoft.AspNetCore.Http;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.ReportModel;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public Task<Report> AddReportAsync(CreateReportModel model, IFormFile file)
        {
            return _reportRepository.AddReportAsync(model, file);
        }
        public Task<Report> UpdateReportAsync(int id, Report report, IFormFile file)
        {
            return _reportRepository.UpdateReportAsync(id, report, file);
        }


        public async Task<IEnumerable<Report>> SearchReportsAsync(
        string userName,
        string? contentSearch,
        string? courseTitleSearch,
        string? issueSearch)
        {

            return await _reportRepository.SearchReportsAsync(userName, contentSearch, courseTitleSearch, issueSearch);
        }
        public async Task<Report> RejectReportAsync(int reportId)
        {
            return await _reportRepository.UpdateReportStatusAsync(reportId, "Reject");
        }

        
        public async Task<IEnumerable<Report>> GetReportsForUserAsync(string userName)
        {
            return await _reportRepository.GetReportsByUserNameAsync(userName);
        }


        

    }
}
