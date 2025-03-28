using Project_Cursus_Group3.Data.Entities;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface IBookmarkServices
    {
        Task<Bookmark> GetByIdAsync(int id);
        Task<IEnumerable<Bookmark>> GetAllAsync();

    }
}
