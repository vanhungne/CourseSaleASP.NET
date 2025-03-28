using Project_Cursus_Group3.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync(string? categoryName); 
        Task<Category> AddAsync(Category category);
        Task<Category> UpdateAsync(int id,Category category);
        Task<Category> DeleteAsync(int id);
    }
}
