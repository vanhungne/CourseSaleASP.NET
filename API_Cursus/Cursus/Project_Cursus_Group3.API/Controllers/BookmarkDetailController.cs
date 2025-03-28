using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Data;
using Project_Cursus_Group3.Data.CustomActionFilters;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.BookmarkDetail;
using Project_Cursus_Group3.Data.ViewModels.BookMarkDTO;
using Project_Cursus_Group3.Service.Interfaces;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    [EnableCors("AllowSpecificOrigins")]


    public class BookmarkDetailController : ControllerBase
    {
        private readonly IBookmarkDetailServices _bookmarkDetailServices;
        private readonly IMapper mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BookmarkDetailController(IBookmarkDetailServices bookmarkDetailServices, IMapper mapper, IUnitOfWork unitOfWork)
        {
            this._bookmarkDetailServices = bookmarkDetailServices;
            this.mapper = mapper;
            this._unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookmarkDetail>>> GetAllBookmarkDetail()
        {
            var bookmarks = await _bookmarkDetailServices.GetAllAsync(User);
            return Ok(mapper.Map<List<BookmarkDetailViewModel>>(bookmarks));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookmarkDetail>> GetById(int id)
        {
            var bookmarkDetail = await _bookmarkDetailServices.GetByIdAsync(id, User);
            if (bookmarkDetail == null)
                return NotFound();

            return Ok(mapper.Map<BookmarkDetailViewModel>(bookmarkDetail));
        }

        [HttpPost]
        [ValidateModel]
        public async Task<ActionResult<BookmarkDetail>> Add([FromBody] CreateBookmarkDetailModel addedBookmark)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.BeginTransaction();
            try
            {
                var bookmarkDetail = mapper.Map<BookmarkDetail>(addedBookmark);
                if (bookmarkDetail == null)
                {
                    return BadRequest("Invalid bookmark data.");
                }

                bookmarkDetail = await _bookmarkDetailServices.AddAsync(bookmarkDetail, User);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return Ok(mapper.Map<BookmarkDetailViewModel>(bookmarkDetail));
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookmarkDetail(int id)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var existingBookmarkDetail = await _bookmarkDetailServices.GetByIdAsync(id, User);
                if (existingBookmarkDetail == null)
                {
                    return NotFound($"BookmarkDetail with ID {id} not found.");
                }

                await _bookmarkDetailServices.DeleteAsync(id, User);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return Ok(mapper.Map<BookmarkDetailViewModel>(existingBookmarkDetail));
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

    }
}

