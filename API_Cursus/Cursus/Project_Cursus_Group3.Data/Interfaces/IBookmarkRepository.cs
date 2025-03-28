using Project_Cursus_Group3.Data.Entities;

namespace Project_Cursus_Group3.Data.Interfaces
{
    public interface IBookmarkRepository
    {
        Task<Bookmark> GetByIdAsync(int id);
        Task<IEnumerable<Bookmark>> GetAllAsync();

    }
}
