using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.ChapterModel;
using Project_Cursus_Group3.Data.ViewModels;
using Project_Cursus_Group3.Data.ViewModels.ChapterDTO;
using Project_Cursus_Group3.Service;
using Project_Cursus_Group3.Service.Interfaces;
using Project_Cursus_Group3.Service.Repository;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]

    public class ChapterController : ControllerBase
    {
        private readonly IChapterServices _chapterServices;
        private readonly ICourseServices _courseServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChapterController(IChapterServices chapterServices, IUnitOfWork unitOfWork, IMapper mapper, ICourseServices courseServices)
        {
            _chapterServices = chapterServices;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _courseServices = courseServices;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Student, Instructor, Admin")]

        public async Task<ActionResult<Category>> GetById(int id)
        {
            var chapter = await _chapterServices.GetChapterByIdAsync(id);
            if (chapter == null)
                return NotFound();

            return Ok(_mapper.Map<ChapterViewModel>(chapter));
        }

        [HttpPost("add-chapter")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> AddChapters([FromBody] List<AddChapterModel> addChapterModels)
        {

            ConvertDefaultValuesToNull(addChapterModels);

            var userNameIdentifierClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userNameIdentifierClaim == null)
            {
                return Unauthorized("User is not logged in.");
            }

            try
            {
                _unitOfWork.BeginTransaction();

                var userName = userNameIdentifierClaim.Value;

                var addedChapters = await _chapterServices.AddChaptersAsync(addChapterModels, userName);
                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return Ok(_mapper.Map<List<AddChapterModel>>(addedChapters));
            }
            catch (KeyNotFoundException ex)
            {
                _unitOfWork.RollbackTransaction();
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _unitOfWork.RollbackTransaction();
                return BadRequest(ex.Message);
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

        [HttpPut("update-chapter")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateChapter([Required] int id, [FromForm] UpdateChapterModel updateChapterModel)
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

                var existingChapter = await _chapterServices.GetChapterByIdAsync(id);

                if (existingChapter == null)
                {
                    throw new KeyNotFoundException($"Chapter with Id {id} not found.");
                }

                var course = await _courseServices.GetByIdAllStatusAsync(updateChapterModel.CourseId.Value);

                if ( course.Username != usernameFromToken)
                {
                    return Unauthorized("You are not authorized to update this chapter.");
                }

                _unitOfWork.BeginTransaction();

                await _chapterServices.UpdateChapterAsync(id, updateChapterModel);

                var updatedChapter = await _chapterServices.GetChapterByIdAsync(id);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return Ok(_mapper.Map<ChapterViewModel>(updatedChapter));
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

        [HttpGet("view-chapter")]
        [Authorize(Roles = "Student, Instructor, Admin")]
        public async Task<IActionResult> ViewAcceptedChapters(string? searchContent, string? sortBy, bool ascending, int? pageNumber, int? pageSize, int? filterDuration)
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

                var userName = userNameIdentifierClaim.Value;

                var chapters = await _chapterServices.ViewActiveChaptersAsync(userName, searchContent, sortBy, ascending, pageNumber, pageSize, filterDuration);
                return Ok(chapters);
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

        [HttpDelete("delete-chapter")]
        [Authorize(Roles = "Instructor, Admin")]
        public async Task<IActionResult> DeleteChapter(int id)
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

                var existingChapter = await _chapterServices.GetChapterByIdAsync(id);

                if (existingChapter == null)
                {
                    throw new KeyNotFoundException($"Chapter with Id {id} not found.");
                }

                var course = await _courseServices.GetByIdAllStatusAsync(existingChapter.CourseId.Value);

                if (course == null || course.Username != usernameFromToken)
                {
                    return Unauthorized("You are not authorized to delete this chapter.");
                }

                var deletedChapter = await _chapterServices.DeleteChapterAsync(id);
                return Ok(_mapper.Map<ChapterViewModel>(deletedChapter));
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

        private void ConvertDefaultValuesToNull(List<AddChapterModel> addChapterModels)
        {
            foreach (var model in addChapterModels)
            {
                if (model.ChapterTitle == "string")
                    model.ChapterTitle = null;

                if (model.SubDescription == "string")
                    model.SubDescription = null;

                if (model.Description == "string")
                    model.Description = null;

                if (model.CourseId == 0)
                    model.CourseId = null;

                if (model.Process == 0)
                    model.Process = null;

                if (model.Duration == 0)
                    model.Duration = null;
            }
        }
    }
}
