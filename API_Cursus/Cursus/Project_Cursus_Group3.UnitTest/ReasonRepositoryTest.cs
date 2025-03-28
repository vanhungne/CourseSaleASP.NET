using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model;
using Project_Cursus_Group3.Data.Model.ChapterModel;
using Project_Cursus_Group3.Data.Model.ReasonModel;
using Project_Cursus_Group3.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Tests.Repository
{
    [TestFixture]
    public class ReasonRepositoryTest
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IConfiguration> _configurationMock;
        private CursusDbContext _dbContext;
        private ReasonRepository _reasonRepository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<CursusDbContext>()
                .UseInMemoryDatabase(databaseName: "CursusDB")
                .Options;
            _dbContext = new CursusDbContext(options);

            _mapperMock = new Mock<IMapper>();
            _configurationMock = new Mock<IConfiguration>();

            _reasonRepository = new ReasonRepository(_dbContext, _mapperMock.Object, _configurationMock.Object);

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
                    UserName = "instructor2",
                    RoleId = 2,
                    Password = "password2",
                    Email = "instructor1@example.com",
                    PhoneNumber = "1234567890",
                    Address = "123 Main St",
                    FullName = "Instructor One",
                    CreatedDate = DateTime.Now,
                    Avatar = "avatar1.png",
                    DOB = new DateTime(1980, 1, 1),
                    Comment = "Top instructor",
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
                    CourseTitle = "Biology Basics",
                    CourseCode = "BL101",
                    Description = "Learn the basics of Biology.",
                    Discount = 20.0,
                    Level = 1,
                    IsComment = true,
                    CreatedDate = DateTime.Now,
                    TotalEnrollment = 100,
                    Status = "Active",
                    Image = "image2.png",
                    Price = 199.99,
                    ShortDescription = "Basic Biology course"
                },
                new Course
                {
                    CourseId = 3,
                    Username = "instructor1",
                    CategoryId = 1,
                    CourseTitle = "Baldy's Basics in Math and Education",
                    CourseCode = "SomeThing2012",
                    Description = "What",
                    Discount = 99.0,
                    Level = 3,
                    IsComment = true,
                    CreatedDate = DateTime.Now,
                    TotalEnrollment = 69420,
                    Status = "Inactive",
                    Image = "image2.png",
                    Price = 199.99,
                    ShortDescription = "Basic Bish"
                }
            });

            _dbContext.Reason.AddRange(new List<Reason>
            {
                new Reason { ReasonId = 1, CourseId = 3, Content = "Terrible reason, don't accept this", Status = "Pending"}
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
        public void UpdateAsync_ShouldThrowKeyNotFoundException_WhenReasonDoesNotExist()
        {
            var updateModel = new UpdateReasonModel { Content = "Accept course pls bby xoxo" };

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _reasonRepository.UpdateAsync(99, updateModel));
        }

        [Test]
        public async Task UpdateAsync_Success_ReasonContentIsUpdated()
        {
            var reasonId = 1;
            var updateModel = new UpdateReasonModel { Content = "Accept course pls bby xoxo" };

            await _reasonRepository.UpdateAsync(reasonId, updateModel);

            var sentReason = await _reasonRepository.GetByIdAsync(1);
            Assert.AreEqual(updateModel.Content, sentReason.Content);
            //Assert.AreEqual(courseId, sentReason.CourseId);
        }
    }
}
