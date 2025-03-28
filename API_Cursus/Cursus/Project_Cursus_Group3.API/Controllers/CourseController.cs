using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using Project_Cursus_Group3.Data;
using Project_Cursus_Group3.Data.CustomActionFilters;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model;
using Project_Cursus_Group3.Data.Model.CourseModel;
using Project_Cursus_Group3.Data.Model.ReasonModel;
using Project_Cursus_Group3.Data.ViewModels;
using Project_Cursus_Group3.Data.ViewModels.CourseDTO;
using Project_Cursus_Group3.Data.ViewModels.PagingDTO;
using Project_Cursus_Group3.Service;
using Project_Cursus_Group3.Service.Interfaces;
using Project_Cursus_Group3.Service.Repository;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]

    public class CourseController : ControllerBase
    {
        private readonly ICourseServices _courseServices;
        private readonly IMapper mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPurchasedCourseServices _purchasedCourseServices;

        public CourseController(ICourseServices courseServices, IMapper mapper, IUnitOfWork unitOfWork, IPurchasedCourseServices puchasedCourseServices)
        {
            _courseServices = courseServices;
            this.mapper = mapper;
            _unitOfWork = unitOfWork;
            _purchasedCourseServices = puchasedCourseServices;
        }

        [HttpPut("{id}")]
        [ValidateModel]
        [Authorize(Roles = "Instructor")]

        public async Task<IActionResult> Update(int id, [FromForm] UpdateCourseModel updateCourse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.BeginTransaction();
            try
            {
                var existingCourse = await _courseServices.GetByIdAsync(id);
                if (existingCourse == null)

                {
                    return NotFound($"Course with ID {id} not found.");
                }
          
               //mapper.Map(updateCourse, existingCourse);
               var updatedCourse = await _courseServices.UpdateAsync(id, updateCourse);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();
                return Ok(mapper.Map<CourseViewModel>(updatedCourse));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ValidateModel]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id, [FromQuery] string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                return BadRequest("Reason is required for deleting the course.");
            }

            _unitOfWork.BeginTransaction();
            try
            {
                var existingCourse = await _courseServices.GetByIdAsync(id);
                if (existingCourse == null)
                {
                    return NotFound($"Course with ID {id} not found.");
                }

                await _courseServices.DeleteAsync(id, reason);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                var responseMessage = new
                {
                    message = "The request was successful, please wait for admin to respond",
                    course = mapper.Map<CourseViewModel>(existingCourse)
                };

                return Ok(responseMessage);
            }
            catch (KeyNotFoundException ex)
            {
                _unitOfWork.RollbackTransaction();
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("Courses-active")]
        [Authorize(Roles = "Student, Instructor, Admin")]
        [AllowAnonymous]

        public async Task<ActionResult<IEnumerable<CourseViewModel>>> GetAll([FromQuery] CourseSearchOptions search, int page = 1, int pageSize = 20)
        {
            var courses = await _courseServices.GetAllCoursesActive(search);
            if (courses == null || !courses.Any())
            {
                return NotFound("No active courses found");
            }
            var pagedCourses = courses.ToPagedList(page, pageSize);
            var response = new PagingModel<CourseViewGET>
            {
                CurrentPage = page,
                TotalPages = pagedCourses.PageCount,
                PageSize = pageSize,
                TotalCount = pagedCourses.TotalItemCount,
                Items = pagedCourses.ToList()
            };
            return Ok(response);
        }

        [HttpGet("get-by-id-or-code")]
        [Authorize(Roles = "Student, Instructor, Admin")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseViewModel>> GetByIdOrCode([FromQuery] int? id = null, [FromQuery] string code = null)
        {
            if (id == null && string.IsNullOrEmpty(code))
            {
                return BadRequest("Either id or code must be provided.");
            }
            try
            {
                var course = await _courseServices.GetByIdOrCodeAsync(id, code);

                if (course == null)
                {
                    return NotFound($"Course with {(id != null ? $"ID {id}" : $"Code {code}")} not found.");
                }

                return Ok(mapper.Map<CourseViewModel>(course));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }


        [HttpPost]
        [Authorize(Roles = "Instructor")]

        public async Task<IActionResult> CreateCourse([FromForm] CreateCourseModel createCourseModel)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("user is not logged in.");
            }

            var nameidentifierclaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (nameidentifierclaim == null)
            {
                return Unauthorized("user identity (nameidentifier) is not available in the token.");
            }

            var nameidentifier = nameidentifierclaim.Value;

            if (!User.IsInRole("Instructor"))
            {
                return Forbid("You do not have permission to create a course.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _courseServices.AddAsync(createCourseModel, nameidentifier);
                return Ok(mapper.Map<CourseViewModel>(result));
            }

            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpPost("ResubmitCourse/{id}")]
        [ValidateModel]
        public async Task<IActionResult> ResubmitCourse(int id, [FromForm] ResubmitCourseModel reasonContent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _unitOfWork.BeginTransaction();

                var existingCourse = await _courseServices.GetInactiveByIdAsync(id);
                if (existingCourse == null)

                {
                    return NotFound($"Course with ID {id} not found.");
                }

                await _courseServices.ResubmitCourse(id, reasonContent);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();
               
                var responseMessage = new
                {
                    message = "The request was successful, please wait for admin to respond",
                    course = mapper.Map<CourseViewModel>(existingCourse)
                };

                return Ok(responseMessage);
            }
            catch (KeyNotFoundException ex)
            {
                //   _unitOfWork.RollbackTransaction();
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                //    _unitOfWork.RollbackTransaction();
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPut("ConfirmCourse/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmCourse(int id, [FromForm] ConfirmCourseModel confirmCourseModel)
        {

            if (confirmCourseModel == null)
            {
                return NotFound("Confirm is required");

            }
            var course = await _courseServices.ConfirmCourse(id, confirmCourseModel.Confirm);
            _unitOfWork.SaveChanges();
            _unitOfWork.CommitTransaction();

            return Ok(mapper.Map<CourseViewModel>(course));


        }
        [HttpGet("view-enrolled")]
        [Authorize(Roles = "Student")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEnrolledCourses(
            string sortByCourseTitleOrLevel = null,
            bool isAscending = true,
            string filterByInstructor = null,
            string searchByCourseTitle = null,
            int pageNumber = 1,
            int pageSize = 10
)
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

            var nameIdentifier = nameIdentifierClaim.Value;

            var courseIds = await _purchasedCourseServices.GetEnrolledCourseIdsByUserNameAsync(nameIdentifier);

            if (courseIds == null || !courseIds.Any())
            {
                return NotFound("User has no enrolled courses.");
            }

            var enrolledCourses = await _courseServices.GetByIdAsync(courseIds);

            if (enrolledCourses == null || !enrolledCourses.Any())
            {
                return NotFound("No courses found for the enrolled IDs.");
            }

            if (!string.IsNullOrEmpty(searchByCourseTitle))
            {
                enrolledCourses = enrolledCourses
                    .Where(c => c.CourseTitle.Contains(searchByCourseTitle, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(filterByInstructor))
            {
                enrolledCourses = enrolledCourses.Where(c => c.Username == filterByInstructor).ToList();
            }

            if (string.IsNullOrEmpty(sortByCourseTitleOrLevel))
            {
                sortByCourseTitleOrLevel = "CourseTitle";
            }

            switch (sortByCourseTitleOrLevel)
            {
                case "CourseTitle":
                    enrolledCourses = isAscending
                        ? enrolledCourses.OrderBy(c => c.CourseTitle).ToList()
                        : enrolledCourses.OrderByDescending(c => c.CourseTitle).ToList();
                    break;
                case "Level":
                    enrolledCourses = isAscending
                        ? enrolledCourses.OrderBy(c => c.Level).ToList()
                        : enrolledCourses.OrderByDescending(c => c.Level).ToList();
                    break;
                default:
                    enrolledCourses = isAscending
                        ? enrolledCourses.OrderBy(c => c.CourseTitle).ToList()
                        : enrolledCourses.OrderByDescending(c => c.CourseTitle).ToList();
                    break;
            }

            var totalRecords = enrolledCourses.Count();
            var pagedCourses = enrolledCourses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewCoursesModel = mapper.Map<List<ViewCoursesModel>>(pagedCourses);

            var response = new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Courses = viewCoursesModel
            };

            return Ok(response);
        }

        [HttpGet("revenue-courses")]
        [Authorize(Roles = "Instructor")]
        [AllowAnonymous]

        public async Task<ActionResult<IEnumerable<CourseViewRevenue>>> GetRevenueCourses()
        {
            var userNameFromToken = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var courses = await _courseServices.GetViewRevenuesAsync(userNameFromToken);

            if (courses == null || !courses.Any())
            {
                return NotFound("No courses found");
            }
            
           
            return Ok(courses);
        }
    }
}
