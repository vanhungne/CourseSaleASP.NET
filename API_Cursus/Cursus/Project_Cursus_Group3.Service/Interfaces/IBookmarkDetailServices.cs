using Project_Cursus_Group3.Data.Entities;
using System.Security.Claims;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface IBookmarkDetailServices
    {
        Task<BookmarkDetail> GetByIdAsync(int id, ClaimsPrincipal userClaims);
        Task<IEnumerable<BookmarkDetail>> GetAllAsync(ClaimsPrincipal userClaims);
        Task<BookmarkDetail> AddAsync(BookmarkDetail bookmarkDetail, ClaimsPrincipal userClaims);
        Task<BookmarkDetail> DeleteAsync(int id, ClaimsPrincipal userClaims);
    }
}
