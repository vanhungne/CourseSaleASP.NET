using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.LessonModel;
using Project_Cursus_Group3.Data.ViewModels.Filter;
using Project_Cursus_Group3.Data.ViewModels.LessonDTO;
using Project_Cursus_Group3.Service.Interfaces;
using System.Security.Claims;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [EnableCors("AllowSpecificOrigins")]

    public class LessonController : ControllerBase
    {
        private readonly ILessonServices _lessonServices;
        private readonly IChapterServices _chapterServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LessonController(ILessonServices lessonServices, IUnitOfWork unitOfWork, IMapper mapper, IChapterServices chapterServices)
        {
            _lessonServices = lessonServices;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _chapterServices = chapterServices;
        }

        [HttpPost("add-lesson")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> AddLesson([FromForm] AddLessonModel addLessonModel)
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


                var addedLesson = await _lessonServices.AddLessonAsync(addLessonModel, userName);
                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return Ok(addedLesson);
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
                return StatusCode(500, ex.InnerException);
            }
        }

        [HttpPut("update-lesson/{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateLesson(int id, [FromForm] UpdateLessonModel updateLessonModel)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("User is not logged in.");
                }

                var usernameFromToken = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (usernameFromToken == null)
                {
                    return Unauthorized("Token does not contain a valid UserName.");
                }

                var existingLesson = await _lessonServices.GetLessonByIdAsync(id, User);

                if (existingLesson == null)
                {
                    throw new KeyNotFoundException($"Lesson with Id {id} not found.");
                }

                var chapter = await _chapterServices.GetChapterByIdAsync(updateLessonModel.ChapterId.Value);

                if (chapter.Course.Username != usernameFromToken)
                {
                    return Unauthorized("You are not authorized to update this lesson.");
                }

                _unitOfWork.BeginTransaction();
                //  var lesson = _mapper.Map<Lesson>(updateLessonModel);

                await _lessonServices.UpdateLessonAsync(id, updateLessonModel);

                var updatedLesson = await _lessonServices.GetLessonByIdAsync(id, User);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return Ok(updatedLesson);
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("view-lesson")]
        [Authorize(Roles = "Student, Instructor, Admin")]
        public async Task<IActionResult> ViewAcceptedLessons([FromQuery] List<string>? filterOn,
    [FromQuery] List<string>? filterQuery,
    [FromQuery] string? sortBy,
    [FromQuery] bool isAscending = true,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] int? filterDuration = null)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not logged in.");
            }

            var userNameIdentifierClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userNameIdentifierClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            try
            {
                _unitOfWork.BeginTransaction();

                //var userName = userNameIdentifierClaim.Value;

                //var lessons = await _lessonServices.ViewActiveLessonsAsync(userName, searchContent, sortBy, ascending, pageNumber, pageSize, filterDuration);
                //return Ok(lessons);
                List<FilterCriteria> filters = new List<FilterCriteria>();

                // Ensure the filters are properly aligned between filterOn and filterQuery
                if (filterOn != null && filterQuery != null && filterOn.Count == filterQuery.Count)
                {
                    for (int i = 0; i < filterOn.Count; i++)
                    {
                        filters.Add(new FilterCriteria { FilterOn = filterOn[i], FilterQuery = filterQuery[i] });
                    }
                }
                string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var lessonDomains = await _lessonServices.ViewActiveLessonsAsync(
                    userName,
                    filters,
                    sortBy,
                    isAscending,
                    pageNumber,
                    pageSize,
                    filterDuration
                );

                // Map domain models to DTOs (View Models)
                var lessonDTOs = _mapper.Map<List<LessonViewModel>>(lessonDomains);

                // Return the result as an HTTP response
                return Ok(lessonDTOs);
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

        [HttpDelete("delete-lesson")]
        [Authorize(Roles = "Instructor, Admin")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            try
            {

                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("User is not logged in.");
                }

                var usernameFromToken = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (usernameFromToken == null)
                {
                    return Unauthorized("Token does not contain a valid UserName.");
                }

                var existingLesson = await _lessonServices.GetLessonByIdAsync(id, User);

                if (existingLesson == null)
                {
                    throw new KeyNotFoundException($"Lesson with Id {id} not found.");
                }

                var chapter = await _chapterServices.GetChapterByIdAsync(existingLesson.ChapterId.Value);

                if (chapter == null || chapter.Course.Username != usernameFromToken)
                {
                    return Unauthorized("You are not authorized to delete this lesson.");
                }

                var deletedLesson = await _lessonServices.DeleteLessonAsync(id);
                return Ok(_mapper.Map<LessonViewModel>(deletedLesson));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<BookmarkDetail>> GetLessonById(int id)
        {
            var lesson = await _lessonServices.GetLessonByIdAsync(id, User);
            if (lesson == null)
                return NotFound();

            return Ok(_mapper.Map<LessonViewModel>(lesson));
        }
    }
}

