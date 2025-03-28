using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project_Cursus_Group3.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.LoginModel;
using Project_Cursus_Group3.Data.Repository;
using Project_Cursus_Group3.Data.Data;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Project_Cursus_Group3.UnitTest
{
	[TestFixture]
	public class RegisterTests
	{
		private CursusDbContext _dbContext;
		private EmailSenderRepository _emailSenderRepository;
		private AuthenRepository _authenRepository;
		private IConfiguration _configuration;

		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<CursusDbContext>()
				.UseInMemoryDatabase(databaseName: "CursusDB")
				.Options;

			_dbContext = new CursusDbContext(options);

			_configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			_emailSenderRepository = new EmailSenderRepository(_configuration, _dbContext, null);
			_authenRepository = new AuthenRepository(_configuration, _dbContext, _emailSenderRepository);

			// Seed dữ liệu
			_dbContext.User.Add(new User { UserName = "existingUser", Email = "existing@example.com", PhoneNumber = "1234567890", Password = "hashedPassword", Status = "Active" });
			_dbContext.SaveChanges();
		}

		[TearDown]
		public void TearDown()
		{
			_dbContext.Database.EnsureDeleted();
			_dbContext.Dispose();
		}

		[Test]
		public async Task Register_ExistingEmail_ReturnsEmailAlreadyExists()
		{
			var registerDTO = new RegisterLoginModel
			{
				UserName = "newUser",
				Email = "existing@example.com", // Email đã tồn tại
				PhoneNumber = "0987654321",
				Password = "Password123",
				RoleId = 1
			};

			var result = await _authenRepository.Register(registerDTO);

			Assert.AreEqual("Email already exists", result);
		}

		[Test]
		public async Task Register_ExistingUserName_ReturnsUserNameAlreadyExists()
		{
			var registerDTO = new RegisterLoginModel
			{
				UserName = "existingUser", // UserName đã tồn tại
				Email = "new@gmail.com",
				PhoneNumber = "0987654321",
				Password = "Password123",
				RoleId = 1
			};

			var result = await _authenRepository.Register(registerDTO);

			Assert.AreEqual("UserName already exists", result);
		}

		[Test]
		public async Task Register_ExistingPhoneNumber_ReturnsPhoneNumberAlreadyExists()
		{
			var registerDTO = new RegisterLoginModel
			{
				UserName = "newUser",
				Email = "new@gmail.com",
				PhoneNumber = "1234567890", // PhoneNumber đã tồn tại
				Password = "Password123",
				RoleId = 1
			};

			var result = await _authenRepository.Register(registerDTO);

			Assert.AreEqual("Phone number already exists", result);
		}
		[Test]
		public async Task Register_ValidAdminUser_ReturnsEmailVerificationMessage()
		{
			var registerDTO = new RegisterLoginModel
			{
				UserName = "newUser123",
				Email = "new123@gmail.com",
				PhoneNumber = "0987654321",
				Password = "Password123",
				RoleId = 1
			};

			// Thực hiện đăng ký
			var result = await _authenRepository.Register(registerDTO);

			// Kiểm tra kết quả
			Assert.AreEqual("Pls check email to verify.", result);
		}

		[Test]
		public async Task Register_ValidPendingUser_ReturnsPendingApprovalMessage()
		{
			var registerDTO = new RegisterLoginModel
			{
				UserName = "anotherNewUser",
				Email = "another@example.com",
				PhoneNumber = "0987654322",
				Password = "Password123",
				RoleId = 2 // Người dùng không phải admin
			};

			var result = await _authenRepository.Register(registerDTO);

			Assert.AreEqual("Your account is pending admin approval.", result);
		}

		[Test]
		public async Task Register_ThrowsException_ReturnsInternalServerErrorMessage()
		{
			// Tạo điều kiện giả lập để Register ném lỗi
			var registerDTO = new RegisterLoginModel
			{
				UserName = "errorUser",
				Email = "error@example.com",
				PhoneNumber = "0987654323",
				RoleId = 1
			};

			// Gây lỗi bằng cách mô phỏng tình huống gặp lỗi trong việc lưu dữ liệu
			_dbContext.Database.EnsureDeleted(); // Giả lập lỗi xóa cơ sở dữ liệu
			var result = await _authenRepository.Register(registerDTO);

			Assert.IsTrue(result.StartsWith("Internal server error:"));
		}
	}
}
