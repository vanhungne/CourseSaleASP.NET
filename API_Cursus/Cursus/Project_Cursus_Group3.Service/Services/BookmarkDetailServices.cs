using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Service.Interfaces;
using System.Security.Claims;

namespace Project_Cursus_Group3.Service
{
    public class BookmarkDetailServices : IBookmarkDetailServices
    {
        private readonly IBookmarkDetailRepository _bookmarkDetailRepository;
        public BookmarkDetailServices(IBookmarkDetailRepository bookmarkDetailRepository)
        {
            this._bookmarkDetailRepository = bookmarkDetailRepository;
        }

        public Task<BookmarkDetail> AddAsync(BookmarkDetail bookmarkDetail, ClaimsPrincipal userClaims)
        {
            return _bookmarkDetailRepository.AddAsync(bookmarkDetail, userClaims);
        }

        public Task<BookmarkDetail> DeleteAsync(int id, ClaimsPrincipal userClaims)
        {
            return _bookmarkDetailRepository.DeleteAsync(id, userClaims);
        }

        public Task<IEnumerable<BookmarkDetail>> GetAllAsync(ClaimsPrincipal userClaims)
        {
            return _bookmarkDetailRepository.GetAllAsync(userClaims);
        }

        public Task<BookmarkDetail> GetByIdAsync(int id, ClaimsPrincipal userClaims)
        {
            return _bookmarkDetailRepository.GetByIdAsync(id, userClaims);
        }

    }
}
