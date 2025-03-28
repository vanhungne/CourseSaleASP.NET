using AutoMapper;
using Castle.Core.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.UserModel;
using Project_Cursus_Group3.Data.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.UnitTest
{
    public class UserRepositoryTest
    {
        private CursusDbContext _dbContext;
        private UserRepository _userRepository;
        private Mock<Project_Cursus_Group3.Data.Interfaces.IEmailSender> _mockEmailSender;
        private Mock<IMapper> _mockMapper;
        private Mock<IConfiguration> _mockConfiguration;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CursusDbContext>()
                .UseInMemoryDatabase(databaseName: "UserDB")
                .Options;

            _dbContext = new CursusDbContext(options);
            _mockEmailSender = new Mock<Project_Cursus_Group3.Data.Interfaces.IEmailSender>();
            _mockMapper = new Mock<IMapper>();
            _mockConfiguration = new Mock<IConfiguration>();

            _userRepository = new UserRepository(_dbContext, _mockMapper.Object, _mockConfiguration.Object, _mockEmailSender.Object);
            _dbContext.User.Add(new User
            {
                UserName = "user1",
                RoleId = 1,
                Password = BCrypt.Net.BCrypt.HashPassword("OldPassword"),
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
        public void UpdateProfileAsync_ThrowsArgumentException_WhenUserNotFound()
        {
            var updateModel = new UserProfileUpdateModel { Email = "newemail@example.com" };

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userRepository.UpdateProfileAsync("nonexistentUser", updateModel));
        }

        [Test]
        public async Task UpdateProfileAsync_ThrowsArgumentException_WhenEmailAlreadyExists()
        {
            var updateModel = new UserProfileUpdateModel { Email = "user1@example.com" };

            _dbContext.User.Add(new User
            {
                UserName = "user2",
                RoleId = 1,
                Password = "OldPassword",
                Email = "user2@example.com",
                PhoneNumber = "1234567899",
                Address = "123 Main St",
                FullName = "User One",
                CreatedDate = DateTime.Now,
                Avatar = "avatar1.png",
                DOB = new DateTime(1980, 1, 1),
                Comment = "Initial comment",
                Status = "Active"
            });
            await _dbContext.SaveChangesAsync();

            updateModel.Email = "user2@example.com";

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userRepository.UpdateProfileAsync("user1", updateModel));

            Assert.AreEqual("Email user2@example.com already exists.", ex.Message);
        }

        [Test]
        public async Task UpdateProfileAsync_ThrowsInvalidOperationException_WhenEmailSendFails()
        {
            var updateModel = new UserProfileUpdateModel { Email = "newemail@example.com" };

            _mockEmailSender.Setup(e => e.EmailSendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                            .ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userRepository.UpdateProfileAsync("user1", updateModel));
        }

        [Test]
        public async Task UpdatePasswordAsync_ShouldUpdatePassword_WhenUserExistsAndOldPasswordIsCorrect()
        {

            var updatePasswordModel = new UpdatePasswordModel
            {
                OldPassword = "OldPassword",
                NewPassword = "NewPassword",
                ConfirmPassword = "NewPassword"
            };

            _mockEmailSender.Setup(es => es.GetMailBodyUpdatePassword(It.IsAny<UpdatePasswordModel>())).Returns("Email body");
            _mockEmailSender.Setup(es => es.EmailSendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _userRepository.UpdatePasswordAsync("user1", updatePasswordModel);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify("NewPassword", result.Password));
        }

        [Test]
        public void UpdatePasswordAsync_ShouldThrowArgumentException_WhenUserDoesNotExist()
        {
            // Arrange
            var updatePasswordModel = new UpdatePasswordModel
            {
                OldPassword = "OldPassword",
                NewPassword = "NewPassword",
                ConfirmPassword = "NewPassword"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userRepository.UpdatePasswordAsync("nonexistentuser", updatePasswordModel));
            Assert.AreEqual("User nonexistentuser not found.", ex.Message);
        }

        [Test]
        public void UpdatePasswordAsync_ShouldThrowValidationException_WhenNewPasswordAndConfirmPasswordDoNotMatch()
        { 

            var updatePasswordModel = new UpdatePasswordModel
            {
                OldPassword = "OldPassword",
                NewPassword = "NewPassword",
                ConfirmPassword = "DifferentPassword"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ValidationException>(async () => await _userRepository.UpdatePasswordAsync("user1", updatePasswordModel));
            Assert.AreEqual("New password and confirm password do not match.", ex.Message);
        }

        [Test]
        public void UpdatePasswordAsync_ShouldThrowValidationException_WhenOldPasswordIsIncorrect()
        {

            var updatePasswordModel = new UpdatePasswordModel
            {
                OldPassword = "IncorrectOldPassword",
                NewPassword = "NewPassword",
                ConfirmPassword = "NewPassword"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ValidationException>(async () => await _userRepository.UpdatePasswordAsync("user1", updatePasswordModel));
            Assert.AreEqual("Old password isn't correct.", ex.Message);
        }

        [Test]
        public void UpdatePasswordAsync_ShouldThrowValidationException_WhenEmailSendingFails()
        {

            var updatePasswordModel = new UpdatePasswordModel
            {
                OldPassword = "OldPassword",
                NewPassword = "NewPassword",
                ConfirmPassword = "NewPassword"
            };

            _mockEmailSender.Setup(es => es.GetMailBodyUpdatePassword(It.IsAny<UpdatePasswordModel>())).Returns("Email body");
            _mockEmailSender.Setup(es => es.EmailSendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ValidationException>(async () => await _userRepository.UpdatePasswordAsync("user1", updatePasswordModel));
            Assert.AreEqual("There was an error sending the email. Please try again later.", ex.Message);
        }
    }
}
