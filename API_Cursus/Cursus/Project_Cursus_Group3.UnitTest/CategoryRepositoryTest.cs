using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Project_Cursus_Group3.Data;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Repository;

namespace Project_Cursus_Group3.UnitTest
{
    public class CategoryRepositoryTest
    {
        private CursusDbContext _dbContext;
        private CategoryRepository _categoryRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CursusDbContext>()
                .UseInMemoryDatabase(databaseName: "CursusDB")
                .Options;

            _dbContext = new CursusDbContext(options);
            _categoryRepository = new CategoryRepository(_dbContext, new UnitOfWork(_dbContext));

            _dbContext.Category.AddRange(new List<Category>
            {
                new Category { CategoryId = 1, ParentCategoryId = null, CategoryName = "Category 1", Status = "active" },
                new Category { CategoryId = 2, ParentCategoryId = 1, CategoryName = "Category 2", Status = "active" },
                new Category { CategoryId = 3, ParentCategoryId = null, CategoryName = "Category 3", Status = "inactive" },
                 new Category { CategoryId = 11, ParentCategoryId = null, CategoryName = "Category 11", Status = "active" }

            });
            _dbContext.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsCategory()
        {
            var result = await _categoryRepository.GetByIdAsync(11);

            Assert.IsNotNull(result);
            Assert.AreEqual("Category 11", result.CategoryName);
            Assert.IsNull(result.ParentCategoryId);
        }

        [Test]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            var result = await _categoryRepository.GetByIdAsync(999);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllAsync_ActiveCategories_ReturnsActiveCategories()
        {
            var result = await _categoryRepository.GetAllAsync(null);

            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public async Task AddAsync_ValidCategory_AddsCategory()
        {
            var newCategory = new Category { CategoryId = 4, ParentCategoryId = 1, CategoryName = "Category 4", Status = "Active" };
            await _categoryRepository.AddAsync(newCategory);

            var addedCategory = await _categoryRepository.GetByIdAsync(4);
            Assert.IsNotNull(addedCategory);
            Assert.AreEqual("Category 4", addedCategory.CategoryName);
        }






        [Test]
        public async Task UpdateAsync_NonExistingId_ThrowsKeyNotFoundException()
        {
            var categoryToUpdate = new Category { CategoryName = "Non Existing Category", Status = "Active" };

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _categoryRepository.UpdateAsync(999, categoryToUpdate));
        }



        [Test]
        public async Task DeleteAsync_NonExistingId_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _categoryRepository.DeleteAsync(999));
        }
    }
}
