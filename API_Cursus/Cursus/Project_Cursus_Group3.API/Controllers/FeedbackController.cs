using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Project_Cursus_Group3.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model;
using Project_Cursus_Group3.Data.Model.FeedBackModel;
using Project_Cursus_Group3.Data.Repository;
using Project_Cursus_Group3.Data.ViewModels;
using Project_Cursus_Group3.Data.ViewModels.FeedbackDTO;
using Project_Cursus_Group3.Service;
using Project_Cursus_Group3.Service.Interfaces;
using System.Security.Claims;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]

    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackServices _feedbackServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeedbackController(IFeedbackServices feedbackServices, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _feedbackServices = feedbackServices;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost("Add-Feedback")]
        [Authorize(Roles = "Student")]

        public async Task<IActionResult> AddFeedback([FromForm] AddFeedbackModel addFeedbackModel, IFormFile? attachmentFile)
        {

            var userNameIdentifierClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userNameIdentifierClaim == null)
            {
                return Unauthorized("User is not Logged In!.");
            }

            try
            {
                _unitOfWork.BeginTransaction();

                var userName = userNameIdentifierClaim.Value;

                var addedFeedback = await _feedbackServices.AddFeedbackAsync(addFeedbackModel, attachmentFile, userName);
                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return Ok(_mapper.Map<AddFeedbackModel>(addedFeedback));
            }
            catch (ArgumentNullException ex)
            {
                _unitOfWork.RollbackTransaction();
                return BadRequest(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                _unitOfWork.RollbackTransaction();
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(500, ex.InnerException);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Student, Instructor, Admin")]
        public async Task<IActionResult> GetFeedbackById(int id)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var feedback = await _feedbackServices.GetFeedbackByIdAsync(id);
                if (feedback == null)
                {
                    return NotFound();
                }


                return Ok(_mapper.Map<FeedbackViewModel>(feedback));
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("ViewFeedback")]
        [Authorize(Roles = "Student, Instructor, Admin")]
        public async Task<IActionResult> ViewAcceptedFeedbacks(string? searchContent, string? sortBy, bool ascending, int? filterCourseID)
        {

            var userNameIdentifierClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userNameIdentifierClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            try
            {
                _unitOfWork.BeginTransaction();

                var userName = userNameIdentifierClaim.Value;

                var feedbacks = await _feedbackServices.ViewAcceptedFeedbacksAsync(userName, searchContent, sortBy, ascending, filterCourseID);
                return Ok(feedbacks);
            }
            catch (InvalidOperationException ex)
            {
                _unitOfWork.RollbackTransaction();
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("Update-Feedback")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromForm] UpdateFeedbackModel updateFeedbackModel, IFormFile? attachmentFile)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("user is not logged in.");
                }

                var usernameFromToken = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (usernameFromToken == null)
                {
                    return Unauthorized("Token does not contain a valid UserName.");
                }

                var existingFeedback = await _feedbackServices.GetFeedbackByIdAsync(id);

                if (existingFeedback == null)
                {
                    throw new KeyNotFoundException($"Feedback with Id {id} not found.");
                }

                if (existingFeedback.UserName != usernameFromToken)
                {
                    return Unauthorized("You are not authorized to update this feedback.");
                }


                _unitOfWork.BeginTransaction();

                await _feedbackServices.UpdateFeedbackAsync(id, updateFeedbackModel, attachmentFile);

                var updatedFeedback = await _feedbackServices.GetFeedbackByIdAsync(id);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return Ok(_mapper.Map<FeedbackViewModel>(updatedFeedback));
            }
            catch (ArgumentNullException ex)
            {
                _unitOfWork.RollbackTransaction();
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _unitOfWork.RollbackTransaction();
                return NotFound(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                _unitOfWork.RollbackTransaction();
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("Delete-Feedback")]
        [Authorize(Roles = "Student, Admin")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            try
            {
                var deletedFeedback = await _feedbackServices.DeleteFeedbackAsync(id);
                return Ok(_mapper.Map<FeedbackViewModel>(deletedFeedback));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

