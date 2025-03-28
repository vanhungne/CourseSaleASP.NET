using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Data;
using Project_Cursus_Group3.Data.Repository;
using Project_Cursus_Group3.Data.ViewModels;
using Project_Cursus_Group3.Data.ViewModels.ReasonDTO;
using Project_Cursus_Group3.Service;
using Project_Cursus_Group3.Data.CustomActionFilters;
using Project_Cursus_Group3.Data.Model.ReasonModel;
using Project_Cursus_Group3.Service.Interfaces;
using System.Security.Claims;
using Project_Cursus_Group3.Data.Entities;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]

    public class ReasonController : ControllerBase
    {
        private readonly IReasonServices _reasonServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ReasonController(IReasonServices reasonService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _reasonServices = reasonService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        // PUT api/<ReasonController>/5
        [ValidateModel]
        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateReasonModel updateReason)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                _unitOfWork.BeginTransaction();
                var existingReason = await _reasonServices.GetByIdAsync(id);
                if (existingReason == null)
                {
                    return NotFound($"Reason with ID {id} not found.");
                }
                //_mapper.Map(existingReason, updateReason);

                var reasonUpdated = await _reasonServices.UpdateAsync(id, updateReason);

                var reasonViewModel = _mapper.Map<ReasonViewModel>(reasonUpdated);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();
                return Ok(reasonViewModel);
            }
            catch (ArgumentException ex)
            {
                _unitOfWork.RollbackTransaction();
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _unitOfWork.RollbackTransaction();
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(500, "An error occurred: " + ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var reason = await _reasonServices.DeleteAsync(id);

            if (reason == null)
            {
                return NotFound(new { Message = "Reason not found." });
            }

            var reasonViewModel = _mapper.Map<ReasonViewModel>(reason);

            return Ok(new { Message = "Reason deleted successfully.", Reason = reasonViewModel });
        }


        [HttpGet("GetReasonByUsername")]

        public async Task<IActionResult> GetReasons([FromQuery] string? reasonContent, [FromQuery] string? courseTitle)
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

            var username = nameIdentifierClaim.Value;


            var reasons = await _reasonServices.GetReasonsByUsernameAsync(username, reasonContent, courseTitle);

            if (reasons == null || reasons.Count == 0)
            {
                return NotFound("No reasons found matching the search criteria.");
            }

            return Ok(_mapper.Map<List<ReasonViewModel>>(reasons));
        }
    }
}
