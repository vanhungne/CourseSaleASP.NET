using AutoMapper;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.Report;
using Project_Cursus_Group3.Data.Model.ReportModel;
using Project_Cursus_Group3.Data.ViewModels.Report;
using Project_Cursus_Group3.Data.ViewModels.ReportDTO;
using Project_Cursus_Group3.Service.Interfaces;
using System.Security.Claims;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]

    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;

        public ReportController(IReportService reportService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _reportService = reportService;
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
       
        [HttpPost("add")]
        [Authorize(Roles = "Student")]

        public async Task<IActionResult> AddReport([FromForm] CreateReportModel model, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _unitOfWork.BeginTransaction();
                var report = await _reportService.AddReportAsync(model, file);
                if (report == null)
                {
                    return NotFound(new { message = "Failed to add the report. Please check the details and try again." });
                }
                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                //return Ok(mapper.Map<ReportViewModel>(report));
                return Ok(new { message = "Report added successfully."});
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while adding the report.", error = ex.Message });
            }

        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "Student")]

        public async Task<IActionResult> UpdateReport(int id, [FromForm] UpdateReportModel model, IFormFile file = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _unitOfWork.BeginTransaction();
                var reportToUpdate = new Report
                {
                    Issue = model.Issue,
                    Content = model.Content
                };

                var updatedReport = await _reportService.UpdateReportAsync(id, reportToUpdate, file);
                if (updatedReport == null)
                {
                    return NotFound(new { message = $"Report with ID {id} not found." });
                }

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();
                var reportViewModel = mapper.Map<ReportViewModel>(updatedReport);
                return Ok(new { message = "Report updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                _unitOfWork.RollbackTransaction();
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the report.", error = ex.Message });
            }
        }

        [HttpGet("View")]
        [Authorize(Roles = "Student, Instructor, Admin")]
        public async Task<IActionResult> GetReports(
              [FromQuery] string? contentSearch,
               [FromQuery] string? courseTitleSearch,
             [FromQuery] string? issueSearch)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not logged in.");
            }

            var nameIdentifierClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (nameIdentifierClaim == null)
            {
                return Unauthorized("User identity (nameidentifier) is not available in the token.");
            }

            var userName = nameIdentifierClaim.Value;

            var reports = await _reportService.SearchReportsAsync(userName, contentSearch, courseTitleSearch, issueSearch);

            if (reports == null || !reports.Any())
            {
                return NotFound("No reports found for the current user.");
            }

            return Ok(mapper.Map<List<ViewReportModel>>(reports));
        }


        [HttpPut("{reportId}/reject")]
        [Authorize(Roles = "Student, Admin")]
        public async Task<IActionResult> RejectReport(int reportId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not logged in.");
            }

            var nameIdentifierClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (nameIdentifierClaim == null)
            {
                return Unauthorized("User identity (nameidentifier) is not available in the token.");
            }

            var userName = nameIdentifierClaim.Value;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User is not authenticated.");
            }

            var report = await _reportService.RejectReportAsync(reportId);

            if (report == null)
            {
                return NotFound($"Report with ID {reportId} not found.");
            }
            return Ok("Report has been rejected successfully.");
        }

    }
}