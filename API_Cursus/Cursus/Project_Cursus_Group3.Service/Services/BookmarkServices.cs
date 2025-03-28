using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Service.Interfaces;

namespace Project_Cursus_Group3.Service.Services
{
    public class BookmarkServices : IBookmarkServices
    {
        private readonly IBookmarkRepository _bookmarkRepository;

        public BookmarkServices(IBookmarkRepository bookmarkRepository)
        {
            _bookmarkRepository = bookmarkRepository;
        }

        public Task<IEnumerable<Bookmark>> GetAllAsync()
        {
            return _bookmarkRepository.GetAllAsync();
        }

        public Task<Bookmark> GetByIdAsync(int id)
        {
            return _bookmarkRepository.GetByIdAsync(id);
        }


    }
}
