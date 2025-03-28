using Microsoft.EntityFrameworkCore;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly CursusDbContext _dbContext;
        private readonly IUnitOfWork unitOfWork;

        public CategoryRepository(CursusDbContext context,IUnitOfWork unitOfWork) : base(context)
        {
            _dbContext = context;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await Entities.FirstOrDefaultAsync(x => x.CategoryId == id && x.Status.ToLower() == "Active");
        }


        public async Task<IEnumerable<Category>> GetAllAsync(string? categoryName)
        {
            var query = Entities.Where(x => x.Status.ToLower() == "Active");

            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(x => x.CategoryName.ToLower().Contains(categoryName.ToLower()));
            }

            return await query.ToListAsync();
        }


        public async Task<Category> AddAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            if (category.ParentCategoryId.HasValue)
            {
                var parentCategory = await GetByIdAsync(category.ParentCategoryId.Value);
                if (parentCategory == null)
                {
                    throw new KeyNotFoundException($"ParenCategoryID {category.ParentCategoryId} NotFound.");
                }
            }
            var existCategory = await Entities.FirstOrDefaultAsync(c => c.CategoryName.Equals(category.CategoryName));
            if (existCategory != null)
            {
                throw new Exception($"Category {category.CategoryName} is existed!");
            }

            await Entities.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }



        public async Task<Category> UpdateAsync(int id ,Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            var existingCategory = await GetByIdAsync(id);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with Id {id} not found.");
            }

            if (category.ParentCategoryId.HasValue)
            {
                var parentCategory = await GetByIdAsync(category.ParentCategoryId.Value);
                if (parentCategory == null)
                {
                    throw new KeyNotFoundException($"ParenCategoryID {category.ParentCategoryId} NotFound.");
                }
            }
            existingCategory.CategoryName = category.CategoryName;
            existingCategory.ParentCategoryId = category.ParentCategoryId;
            await _dbContext.SaveChangesAsync();
            return existingCategory;
        }


        public async Task<Category> DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with Id {id} not found.");
            }
            category.Status = "Inactive";
            await _dbContext.SaveChangesAsync();
            return category;
        }

    }
}
