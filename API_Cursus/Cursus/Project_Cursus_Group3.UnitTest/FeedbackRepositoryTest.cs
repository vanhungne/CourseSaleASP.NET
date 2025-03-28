using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.FeedBackModel;
using Project_Cursus_Group3.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.UnitTest
{
    [TestFixture]
    public class FeedbackRepositoryTest
    {
        private CursusDbContext _dbContext;
        private FeedbackRepository _feedbackRepository;
        private Mock<IConfiguration> _mockConfiguration;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CursusDbContext>()
                .UseInMemoryDatabase(databaseName: "CursusDB")
                .Options;

            _dbContext = new CursusDbContext(options);
            _mockConfiguration = new Mock<IConfiguration>();
            _feedbackRepository = new FeedbackRepository(_dbContext, _mockConfiguration.Object);

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

            _dbContext.PurchasedCourse.AddRange(new List<PurchasedCourse>
            {
                new PurchasedCourse { CourseId = 1, UserName = "user3", Status = "Active" },
                new PurchasedCourse { CourseId = 2, UserName = "user2", Status = "Active"}
            });

            _dbContext.Feedback.AddRange(new List<Feedback>
            {
                new Feedback { FeedbackId = 1, UserName = "user1", Content = "Good", Status = "Accept", CourseId = 1, Star = 5 },
                new Feedback { FeedbackId = 2, UserName = "user2", Content = "Bad", Status = "reject", CourseId = 1, Star = 3 }
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
        public async Task GetFeedbackByIdAsync_ExistingId_ReturnsFeedback()
        {
            var result = await _feedbackRepository.GetFeedbackByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Good", result.Content);
        }

        [Test]
        public async Task GetFeedbackByIdAsync_NonExistingId_ReturnsNull()
        {
            var result = await _feedbackRepository.GetFeedbackByIdAsync(999);

            Assert.IsNull(result);
        }

        [Test]
        public async Task AddFeedbackAsync_ValidModel_AddsFeedback()
        {
            var newFeedback = new AddFeedbackModel { CourseId = 1, Content = "Excellent", Star = 5 };
            var mockFile = new Mock<IFormFile>();

            await _feedbackRepository.AddFeedbackAsync(newFeedback, mockFile.Object, "user3");

            var addedFeedback = await _dbContext.Feedback.FirstOrDefaultAsync(f => f.Content == "Excellent");
            Assert.IsNotNull(addedFeedback);
            Assert.AreEqual("Excellent", addedFeedback.Content);
        }

        [Test]
        public async Task AddFeedbackAsync_NullModel_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _feedbackRepository.AddFeedbackAsync(null, null, "user3"));
        }

        [Test]
        public async Task AddFeedbackAsync_CourseNotFound_ThrowsInvalidOperationException()
        {
            var newFeedback = new AddFeedbackModel { CourseId = 999, Content = "Test", Star = 4 };
            var mockFile = new Mock<IFormFile>();

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _feedbackRepository.AddFeedbackAsync(newFeedback, mockFile.Object, "user3"));
        }

        [Test]
        public async Task AddFeedbackAsync_CourseIsNotCommentable_ThrowsInvalidOperationException()
        {
            var newFeedback = new AddFeedbackModel { CourseId = 2, Content = "Test", Star = 4 };
            var mockFile = new Mock<IFormFile>();

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _feedbackRepository.AddFeedbackAsync(newFeedback, mockFile.Object, "user3"));
        }

        [Test]
        public async Task AddFeedbackAsync_UserNotPurchasedCourse_ThrowsInvalidOperationException()
        {
            var newFeedback = new AddFeedbackModel { CourseId = 1, Content = "Test", Star = 4 };
            var mockFile = new Mock<IFormFile>();

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _feedbackRepository.AddFeedbackAsync(newFeedback, mockFile.Object, "user4"));
        }

        [Test]
        public async Task DeleteFeedbackAsync_ValidId_RejectsFeedback()
        {
            var feedback = await _feedbackRepository.DeleteFeedbackAsync(1);

            Assert.AreEqual("Reject", feedback.Status);
        }

        [Test]
        public void DeleteFeedbackAsync_NonExistingId_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _feedbackRepository.DeleteFeedbackAsync(999));
        }

        [Test]
        public void DeleteFeedbackAsync_AlreadyRejected_ThrowsInvalidOperationException()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _feedbackRepository.DeleteFeedbackAsync(2));

            //var feedback = await _feedbackRepository.DeleteFeedbackAsync(2);

            //Assert.AreEqual("Reject", feedback.Status);
        }

        [Test]
        public async Task UpdateFeedbackAsync_ValidUpdate_UpdatesFeedback()
        {
            var updateModel = new UpdateFeedbackModel { Content = "Updated Content", Star = 4 };
            var mockFile = new Mock<IFormFile>();

            await _feedbackRepository.UpdateFeedbackAsync(1, updateModel, mockFile.Object);

            var updatedFeedback = await _feedbackRepository.GetFeedbackByIdAsync(1);

            Assert.AreEqual("Updated Content", updatedFeedback.Content);
            Assert.AreEqual(4, updatedFeedback.Star);
        }

        [Test]
        public void UpdateFeedbackAsync_NullModel_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _feedbackRepository.UpdateFeedbackAsync(1, null, null));
        }

        [Test]
        public void UpdateFeedbackAsync_NonExistingId_ThrowsKeyNotFoundException()
        {
            var updateModel = new UpdateFeedbackModel { Content = "Test", Star = 4 };
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _feedbackRepository.UpdateFeedbackAsync(999, updateModel, null));
        }

        [Test]
        public async Task ViewAcceptedFeedbacksAsync_ValidUser_ReturnsFeedback()
        {
            var feedbacks = await _feedbackRepository.ViewAcceptedFeedbacksAsync("user1");

            Assert.IsNotNull(feedbacks);
            Assert.IsNotEmpty(feedbacks);
            Assert.AreEqual("user1", feedbacks[0].UserName);
        }

        [Test]
        public void ViewAcceptedFeedbacksAsync_NoFeedback_ThrowsInvalidOperationException()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _feedbackRepository.ViewAcceptedFeedbacksAsync("user3"));
        }

        [Test]
        public async Task ViewAcceptedFeedbacksAsync_WithFilters_ReturnsFilteredResults()
        {
            var feedbacks = await _feedbackRepository.ViewAcceptedFeedbacksAsync("user1", "good", "star", false);

            Assert.IsNotNull(feedbacks);
            Assert.AreEqual(1, feedbacks.Count);
            Assert.AreEqual(5, feedbacks[0].Star);
        }
    }
}
