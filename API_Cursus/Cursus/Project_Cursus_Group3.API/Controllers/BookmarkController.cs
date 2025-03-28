using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.ViewModels.BookMarkDTO;
using Project_Cursus_Group3.Service.Interfaces;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    [EnableCors("AllowSpecificOrigins")]

    public class BookmarkController : ControllerBase
    {
        private readonly IBookmarkServices _bookmarkServices;
        private readonly IMapper mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BookmarkController(IBookmarkServices bookmarkServices, IMapper mapper, IUnitOfWork unitOfWork)
        {
            this._bookmarkServices = bookmarkServices;
            this.mapper = mapper;
            this._unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bookmark>>> GetAllBookmark()
        {
            var bookmarks = await _bookmarkServices.GetAllAsync();
            return Ok(mapper.Map<List<BookmarkViewModel>>(bookmarks));
        }

        [HttpGet("/bookmark-{id}")]
        public async Task<ActionResult<Bookmark>> GetById(int id)
        {
            var bookmark = await _bookmarkServices.GetByIdAsync(id);
            if (bookmark == null)
                return NotFound();

            return Ok(mapper.Map<BookmarkViewModel>(bookmark));
        }


    }
}
