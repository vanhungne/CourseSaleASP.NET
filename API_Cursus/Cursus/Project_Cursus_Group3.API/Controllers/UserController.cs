using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using Project_Cursus_Group3.Data;
using Project_Cursus_Group3.Data.CustomActionFilters;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.UserModel;
using Project_Cursus_Group3.Data.ViewModels.PagingDTO;
using Project_Cursus_Group3.Data.ViewModels.UserDTO;
using Project_Cursus_Group3.Service.Interfaces;
using System.Security.Claims;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]

    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserController(IUserServices userServices, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userServices = userServices;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        [HttpGet("GetUserProfile")]
        [Authorize(Roles = "Admin,Instructor,Student")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userNameFromToken = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userNameFromToken))
            {
                return Unauthorized(new { message = "User token is invalid." });
            }

            var user = await _userServices.GetUserByUserName(userNameFromToken);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var userViewModel = _mapper.Map<UserViewModel>(user);
            return Ok(userViewModel);
        }



        [HttpGet("Get-all")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<IEnumerable<UserViewGet>>> GetAllUsers([FromQuery] UserSearchOptions searchOptions,
                                                                   [FromQuery] bool sortByDOB = false, [FromQuery]
                                                                                      bool ascending = true,
                                                                               int page = 1, int pageSize = 20)
        {
            var users = await _userServices.GetAllUser(searchOptions, sortByDOB, ascending);
            if (users == null || !users.Any())
            {
                return NotFound();
            }
            var pageUser = users.ToPagedList(page, pageSize);
            var response = new PagingModel<UserViewGet>
            {
                CurrentPage = page,
                TotalPages = pageUser.PageCount,
                PageSize = pageSize,
                TotalCount = pageUser.TotalItemCount,
                Items = pageUser.ToList()
            };

            return Ok(response);
        }

        [HttpPut("Update-Profile/{userName}")]
        [ValidateModel]
        [Authorize(Roles = "Instructor,Student")]
        public async Task<IActionResult> UpdateProfile(string userName, [FromForm] UserProfileUpdateModel userProfileUpdateModel)
        {
            var userNameFromToken = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userNameFromToken != userName)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You are not allowed to update other people's profiles." });
            }

            var existingUser = await _userServices.GetUserByUserName(userName);
            if (existingUser == null)
            {
                return NotFound(new { message = "User not found." });
            }

            try
            {
                _unitOfWork.BeginTransaction();

                var user = _mapper.Map<User>(userProfileUpdateModel);

                user = await _userServices.UpdateProfileAsync(userName, userProfileUpdateModel);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                if (!string.IsNullOrEmpty(userProfileUpdateModel.Email) && userProfileUpdateModel.Email != existingUser.Email)
                {
                    return Ok("Please check your email for verification.");

                }
                else
                {
                    return Ok("Success Update");
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPut("updatePassword")]
        [ValidateModel]
        [Authorize(Roles = "Instructor,Student")]
        public async Task<IActionResult> UpdatePassword([FromForm] UpdatePasswordModel updatePassword)
        {
            var userNameFromToken = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userServices.GetUserByUserName(userNameFromToken);
            if (existingUser == null)
            {
                return NotFound(new { message = "User not found." });
            }
            if (userNameFromToken != existingUser.UserName)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You are not allowed to update other people's profiles." });
            }
            try
            {
                _unitOfWork.BeginTransaction();

                await _userServices.UpdatePasswordAsync(userNameFromToken, updatePassword);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return Ok("Password change successfully. Check your mail pls <3");
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpDelete("delete-user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string userName, [FromBody] string comment)
        {
            if (string.IsNullOrEmpty(comment))
            {
                return BadRequest("Comment is required for deleting the course.");
            }

            _unitOfWork.BeginTransaction();

            try
            {
                var user = await _userServices.DeleteUser(userName, comment);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();
                return Ok(_mapper.Map<UserViewModel>(user));
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

    }
}
