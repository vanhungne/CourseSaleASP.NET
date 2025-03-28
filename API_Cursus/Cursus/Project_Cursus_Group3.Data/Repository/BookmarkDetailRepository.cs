using Microsoft.EntityFrameworkCore;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using System.Security.Claims;

namespace Project_Cursus_Group3.Data.Repository
{
    public class BookmarkDetailRepository : Repository<BookmarkDetail>, IBookmarkDetailRepository
    {
        private readonly CursusDbContext _dbContext;
        public BookmarkDetailRepository(CursusDbContext context) : base(context)
        {
            this._dbContext = context;
        }
        public async Task<BookmarkDetail> GetByIdAsync(int id, ClaimsPrincipal userClaims)
        {
            var username = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //viewdetail của bookmark là xem khóa học
            return await Entities.Include(x => x.Course).FirstOrDefaultAsync(x => x.Course.CourseId == id && x.Status.ToLower() == "Active" && x.Bookmark.UserName == username);
        }

        public async Task<IEnumerable<BookmarkDetail>> GetAllAsync(ClaimsPrincipal userClaims)
        {
            var username = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return await Entities.Include(x => x.Course).Where(x => x.Status.ToLower() == "Active" && x.Bookmark.UserName == username).ToListAsync();
        }

        public async Task<BookmarkDetail> AddAsync(BookmarkDetail bookmarkDetail, ClaimsPrincipal userClaims)
        {
            if (bookmarkDetail == null)
            {
                throw new ArgumentNullException(nameof(bookmarkDetail));
            }
            //Chosen course not active cannot add in the bookmark

            //if (bookmarkDetail.Course.Status != "Active")
            //{
            //    throw new KeyNotFoundException($"BookmarkDetail {bookmarkDetail.CourseId} NotFound.");
            //}

            var username = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userBookmark = await _dbContext.Bookmark
                .Include(b => b.BookmarkDetails) // Include existing bookmark details if needed
                .FirstOrDefaultAsync(b => b.UserName == username);

            if (userBookmark == null)
            {
                throw new InvalidOperationException("User's bookmark not found.");
            }

            var course = await _dbContext.Course.FindAsync(bookmarkDetail.CourseId);
            if (course == null)
            {
                throw new KeyNotFoundException($"Course with ID {bookmarkDetail.CourseId} does not exist.");
            }

            if (course.Status != "Active")
            {
                throw new InvalidOperationException($"Course with ID {bookmarkDetail.CourseId} is not active.");
            }

            var existingBookmarkDetail = userBookmark.BookmarkDetails
                                        .FirstOrDefault(bd => bd.CourseId == bookmarkDetail.CourseId);

            if (existingBookmarkDetail != null)
            {
                throw new InvalidOperationException($"Course with ID {bookmarkDetail.CourseId} is already in the bookmark.");
            }

            bookmarkDetail.BookmarkId = userBookmark.BookmarkId;
            bookmarkDetail.Status = "Active";
            await Entities.AddAsync(bookmarkDetail);
            await _dbContext.SaveChangesAsync();
            return bookmarkDetail;
        }

        public async Task<BookmarkDetail> DeleteAsync(int id, ClaimsPrincipal userClaims)
        {
            var username = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var bookmarkDetail = await GetByIdAsync(id, userClaims);
            if (bookmarkDetail == null)
            {
                throw new KeyNotFoundException($"Bookmark not have course with Id {id} not found.");
            }
            bookmarkDetail.Status = "Inactive";
            await _dbContext.SaveChangesAsync();
            return bookmarkDetail;
        }

    }
}
