using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.UnitTest
{
    public class AdminRepositoryTest
    {
        private CursusDbContext _dbContext;
        private AdminRepository _adminRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CursusDbContext>()
                .UseInMemoryDatabase(databaseName: "CursusDB")
                .Options;

            _dbContext = new CursusDbContext(options);
            _adminRepository = new AdminRepository(_dbContext);

            _dbContext.Role.AddRange(new List<Role>
            {
                new Role { RoleId = 1, RoleName = "Student" },
                new Role { RoleId = 2, RoleName = "Instructor" },
                new Role { RoleId = 3, RoleName = "Admin" }
            });

            _dbContext.User.Add(new User
            {
                UserName = "user1",
                RoleId = 1,
                Password = "password1",
                Email = "user1@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Main St",
                FullName = "User One",
                CreatedDate = DateTime.Now,
                Avatar = "avatar1.png",
                DOB = new DateTime(1980, 1, 1),
                Comment = "Initial comment",
                Status = "Active"
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
        public void ToggleUserStatusAsync_ThrowsArgumentException_WhenCommentIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _adminRepository.ToggleUserStatusAsync("user1", false, ""));
        }

        [Test]
        public void ToggleUserStatusAsync_ThrowsKeyNotFoundException_WhenUserNotFound()
        {
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _adminRepository.ToggleUserStatusAsync("nonexistentUser", false, "Inactive due to inactivity"));
        }

        [Test]
        public async Task ToggleUserStatusAsync_ThrowsInvalidOperationException_WhenStatusUnchanged()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _adminRepository.ToggleUserStatusAsync("user1", true, "Already active"));

            Assert.AreEqual("The Status is already active.", ex.Message);
        }

        [Test]
        public async Task ToggleUserStatusAsync_ChangesStatusToInactive()
        {
            await _adminRepository.ToggleUserStatusAsync("user1", false, "Inactive due to inactivity");

            var user = await _dbContext.User.FindAsync("user1");
            Assert.AreEqual("inactive", user.Status);
            Assert.AreEqual("Inactive due to inactivity", user.deleteComment);
        }

        [Test]
        public async Task ToggleUserStatusAsync_ChangesStatusToActive()
        {
            var user = await _dbContext.User.FindAsync("user1");
            user.Status = "Inactive";
            await _dbContext.SaveChangesAsync();

            await _adminRepository.ToggleUserStatusAsync("user1", true, "Reactivated");

            user = await _dbContext.User.FindAsync("user1");
            Assert.AreEqual("active", user.Status);
            Assert.AreEqual("Reactivated", user.deleteComment);
        }
    }
}
