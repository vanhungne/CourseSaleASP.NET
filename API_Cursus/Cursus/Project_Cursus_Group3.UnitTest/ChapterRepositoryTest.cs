using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.ChapterModel;
using Project_Cursus_Group3.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.UnitTest
{
    public class ChapterRepositoryTest
    {
        private CursusDbContext _dbContext;
        private ChapterRepository _chapterRepository;
        private Mock<IConfiguration> _mockConfiguration;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CursusDbContext>()
                .UseInMemoryDatabase(databaseName: "CursusDB")
                .Options;

            _dbContext = new CursusDbContext(options);
            _mockConfiguration = new Mock<IConfiguration>();
            _chapterRepository = new ChapterRepository(_dbContext, _mockConfiguration.Object);

            _dbContext.Role.AddRange(new List<Role>
            {
                new Role { RoleId = 1, RoleName = "Student" },
                new Role { RoleId = 2, RoleName = "Instructor" },
                new Role { RoleId = 3, RoleName = "Admin" }
            });

            _dbContext.User.AddRange(new List<User>
            {
                new User
                {
                    UserName = "instructor1",
                    RoleId = 2,
                    Password = "password1",
                    Email = "instructor1@example.com",
                    PhoneNumber = "1234567890",
                    Address = "123 Main St",
                    FullName = "Instructor One",
                    CreatedDate = DateTime.Now,
                    Avatar = "avatar1.png",
                    DOB = new DateTime(1980, 1, 1),
                    Comment = "Top instructor",
                    Status = "Active"
                },
                new User
                {
                    UserName = "user3",
                    RoleId = 1,
                    Password = "password3",
                    Email = "user3@example.com",
                    PhoneNumber = "0987654321",
                    Address = "456 Elm St",
                    FullName = "User Three",
                    CreatedDate = DateTime.Now,
                    Avatar = "avatar3.png",
                    DOB = new DateTime(1990, 5, 20),
                    Comment = "Regular student",
                    Status = "Active"
                }
            });

            _dbContext.Category.AddRange(new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Tech", Status = "Active" },
                new Category { CategoryId = 2, CategoryName = "Science", Status = "Active" }
            });

            _dbContext.Course.AddRange(new List<Course>
            {
                new Course
                {
                    CourseId = 1,
                    Username = "instructor1",
                    CategoryId = 1,
                    CourseTitle = "C# Basics",
                    CourseCode = "CS101",
                    Description = "Learn the basics of C#.",
                    Discount = 10.0,
                    Level = 1,
                    IsComment = true,
                    CreatedDate = DateTime.Now,
                    TotalEnrollment = 100,
                    Status = "Active",
                    Image = "image1.png",
                    Price = 99.99,
                    ShortDescription = "Basic C# course"
                },
                new Course
                {
                    CourseId = 2,
                    Username = "instructor1",
                    CategoryId = 2,
                    CourseTitle = "Advanced Physics",
                    CourseCode = "PH202",
                    Description = "Advanced topics in Physics.",
                    Discount = 15.0,
                    Level = 2,
                    IsComment = false,
                    CreatedDate = DateTime.Now,
                    TotalEnrollment = 50,
                    Status = "Active",
                    Image = "image2.png",
                    Price = 199.99,
                    ShortDescription = "Physics course for advanced learners"
                }
            });

            _dbContext.Chapter.Add(new Chapter { ChapterId = 1, CourseId = 1, ChapterTitle = "Intro to C#", Status = "Active", Lessons = new List<Lesson>() });
            _dbContext.Chapter.Add(new Chapter { ChapterId = 2, CourseId = 1, ChapterTitle = "Intro to Java", Status = "Active",
                Lessons = new List<Lesson>
        {
            new Lesson { LessonId = 1, ChapterId = 1, LessonTitle = "Lesson 1", Status = "Active" }
        }
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
        public void AddChapterAsync_CourseDoesNotExist_ThrowsKeyNotFoundException()
        {
            var model = new List<AddChapterModel>
            {
                new AddChapterModel { CourseId = 99, ChapterTitle = "New Chapter" }
            };

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _chapterRepository.AddChaptersAsync(model, "instructor1"));
        }

        [Test]
        public void AddChapterAsync_UserNotAuthorized_ThrowsInvalidOperationException()
        {
            var model = new List<AddChapterModel>
            {
                new AddChapterModel { CourseId = 1, ChapterTitle = "New Chapter" }
            };

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chapterRepository.AddChaptersAsync(model, "user3"));
        }

        [Test]
        public void AddChapterAsync_DuplicateChapterTitle_ThrowsInvalidOperationException()
        {
            var model = new List<AddChapterModel>
            {
                new AddChapterModel { CourseId = 1, ChapterTitle = "Intro to C#" }
            };

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chapterRepository.AddChaptersAsync(model, "instructor1"));
        }

        [Test]
        public async Task AddChapterAsync_Success_ReturnsAddChapterModel()
        {
            var models = new List<AddChapterModel>
    {
            new AddChapterModel
            {
                CourseId = 1,
                ChapterTitle = "Advanced C#",
                SubDescription = "Sub description",
                Description = "Full description",
                Process = 100,
                Duration = 120
            },

            new AddChapterModel
            {
                CourseId = 1,
                ChapterTitle = "Advanced Java",
                SubDescription = "Sub description",
                Description = "Full description",
                Process = 100,
                Duration = 120
            }
        };

            var result = await _chapterRepository.AddChaptersAsync(models, "instructor1");

            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Advanced C#", result[0].ChapterTitle);
            Assert.AreEqual("Advanced Java", result[1].ChapterTitle);
            Assert.AreEqual(4, _dbContext.Chapter.Count());
        }

        [Test]
        public void DeleteChapterAsync_ChapterDoesNotExist_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsAsync<KeyNotFoundException>(() => _chapterRepository.DeleteChapterAsync(99));
        }

        [Test]
        public async Task DeleteChapterAsync_ChapterHasLessons_ThrowsInvalidOperationException()
        {
            var chapter = await _dbContext.Chapter.Include(c => c.Lessons).FirstOrDefaultAsync(c => c.ChapterId == 2);
            Assert.ThrowsAsync<InvalidOperationException>(() => _chapterRepository.DeleteChapterAsync(2));
        }

        [Test]
        public async Task DeleteChapterAsync_Success_DeactivatesChapter()
        {
            var chapter = await _chapterRepository.DeleteChapterAsync(1);

            Assert.AreEqual("Inactive", chapter.Status);
        }

        [Test]
        public async Task GetChapterByIdAsync_ChapterExists_ReturnsChapter()
        {
            var chapter = await _chapterRepository.GetChapterByIdAsync(1);

            Assert.NotNull(chapter);
            Assert.AreEqual(1, chapter.ChapterId);
        }

        [Test]
        public async Task GetChapterByIdAsync_ChapterDoesNotExist_ReturnsNull()
        {
            var chapter = await _chapterRepository.GetChapterByIdAsync(99);

            Assert.IsNull(chapter);
        }

        [Test]
        public void UpdateChapterAsync_ChapterDoesNotExist_ThrowsKeyNotFoundException()
        {
            var updateModel = new UpdateChapterModel { ChapterTitle = "Updated Title" };

            Assert.ThrowsAsync<KeyNotFoundException>(() => _chapterRepository.UpdateChapterAsync(99, updateModel));
        }

        [Test]
        public void UpdateChapterAsync_CourseDoesNotExist_ThrowsInvalidOperationException()
        {
            var updateModel = new UpdateChapterModel { CourseId = 99 };

            Assert.ThrowsAsync<InvalidOperationException>(() => _chapterRepository.UpdateChapterAsync(1, updateModel));
        }

        [Test]
        public void UpdateChapterAsync_DuplicateChapterTitle_ThrowsInvalidOperationException()
        {
            _dbContext.Chapter.Add(new Chapter { ChapterId = 3, CourseId = 1, ChapterTitle = "Advanced C#" });
            _dbContext.SaveChanges();

            var updateModel = new UpdateChapterModel { ChapterTitle = "Advanced C#" };

            Assert.ThrowsAsync<InvalidOperationException>(() => _chapterRepository.UpdateChapterAsync(1, updateModel));
        }

        [Test]
        public async Task UpdateChapterAsync_Success_UpdatesChapter()
        {
            var updateModel = new UpdateChapterModel { ChapterTitle = "Updated Title" };

            await _chapterRepository.UpdateChapterAsync(1, updateModel);

            var updatedChapter = await _dbContext.Chapter.FindAsync(1);
            Assert.AreEqual("Updated Title", updatedChapter.ChapterTitle);
        }

        [Test]
        public async Task ViewActiveChaptersAsync_ReturnsActiveChapters()
        {
            var result = await _chapterRepository.ViewActiveChaptersAsync("instructor1");

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(c => c.Status == "Active"));
        }

        [Test]
        public async Task ViewActiveChaptersAsync_SortsByChapterTitleDescending()
        {
            var result = await _chapterRepository.ViewActiveChaptersAsync("instructor1", sortBy: "ChapterTitle", ascending: false);

            Assert.That(result[0].ChapterTitle, Is.EqualTo("Intro to Java"));
            Assert.That(result[1].ChapterTitle, Is.EqualTo("Intro to C#"));
        }

        [Test]
        public async Task ViewActiveChaptersAsync_FiltersBySearchContent()
        {
            var result = await _chapterRepository.ViewActiveChaptersAsync("instructor1", searchContent: "C#");

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Intro to C#", result[0].ChapterTitle);
        }

        [Test]
        public async Task ViewActiveChaptersAsync_AppliesPaging()
        {
            var result = await _chapterRepository.ViewActiveChaptersAsync("instructor1", pageNumber: 1, pageSize: 1);

            Assert.AreEqual(1, result.Count);
        }
    }
}

