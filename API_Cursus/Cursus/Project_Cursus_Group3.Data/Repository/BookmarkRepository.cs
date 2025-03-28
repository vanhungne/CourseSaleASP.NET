using Microsoft.EntityFrameworkCore;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;

namespace Project_Cursus_Group3.Data.Repository
{
    public class BookmarkRepository : Repository<Bookmark>, IBookmarkRepository
    {
        private readonly CursusDbContext _dbContext;
        public BookmarkRepository(CursusDbContext context) : base(context)
        {
            this._dbContext = context;
        }
        public async Task<Bookmark> GetByIdAsync(int id)
        {
            return await Entities.Include(x => x.User).FirstOrDefaultAsync(x => x.BookmarkId == id);
        }

        public async Task<IEnumerable<Bookmark>> GetAllAsync()
        {
            return await Entities.Include(x => x.User).ToListAsync();
        }


    }
}
