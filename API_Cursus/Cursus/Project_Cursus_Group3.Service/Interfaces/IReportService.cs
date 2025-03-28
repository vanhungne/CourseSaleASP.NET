using Microsoft.AspNetCore.Http;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface IReportService
    {
        Task<Report> AddReportAsync(CreateReportModel model, IFormFile file);
        Task<Report> UpdateReportAsync(int id, Report report, IFormFile file);


        Task<IEnumerable<Report>> GetReportsForUserAsync(string userName);
        Task<IEnumerable<Report>> SearchReportsAsync(
        string userName,
        string? contentSearch,
        string? courseTitleSearch,
        string? issueSearch);
        Task<Report> RejectReportAsync(int reportId);







    }
}
