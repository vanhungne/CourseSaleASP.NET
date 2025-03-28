using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Data;
using Project_Cursus_Group3.Data.ViewModels.UserDTO;
using Project_Cursus_Group3.Service.Interfaces;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableCors("AllowSpecificOrigins")]

    public class AdminController : ControllerBase
    {
        private readonly IAdminServices _adminService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IAdminServices adminService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _adminService = adminService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ToggleUserStatus(string userName, bool isActive, string comment)
        {

            _unitOfWork.BeginTransaction();

            try
            {
                await _adminService.ToggleUserStatusAsync(userName, isActive, comment);
                var user = await _adminService.GetByIdAsync(userName);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return Ok(_mapper.Map<UserViewModel>(user));
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
            catch (InvalidOperationException ex)
            {
                _unitOfWork.RollbackTransaction();
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(500, "An error occurred while toggling user status. " + ex);
            }
        }


        [HttpPost("ConfirmReason")]
        public async Task<IActionResult> ConfirmReason(int courseId, bool isApproved)
        {
            _unitOfWork.BeginTransaction();

            try
            {
                await _adminService.ConfirmReasonAsync(courseId, isApproved);

                string statusMessage = isApproved ? "Accept to stop the course." : "Reject to stop the course.";

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return Ok(new { message = statusMessage });
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
                return StatusCode(500, "An error occurred while confirming reason. " + ex);
            }
        }

    }
}

