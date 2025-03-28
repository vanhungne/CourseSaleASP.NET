using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Project_Cursus_Group3.Data.Repository;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.LoginModel;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Project_Cursus_Group3.Data.Data;
using System.IO;

namespace Project_Cursus_Group3.UnitTest
{
	[TestFixture]
	public class EmailSenderRepositoryTest
	{
		private Mock<ILogger<EmailSenderRepository>> _loggerMock;
		private IConfiguration _configuration;
		private CursusDbContext _dbContext;
		private EmailSenderRepository _emailSenderRepository;

		[SetUp]
		public void Setup()
		{
			// Mock ILogger
			_loggerMock = new Mock<ILogger<EmailSenderRepository>>();

			// Load configuration from appsettings.json
			_configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			// Setup in-memory database
			var options = new DbContextOptionsBuilder<CursusDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb")
				.Options;
			_dbContext = new CursusDbContext(options);

			// Initialize EmailSenderRepository
			_emailSenderRepository = new EmailSenderRepository(_configuration, _dbContext, _loggerMock.Object);
		}

		[TearDown]
		public void TearDown()
		{
			_dbContext.Database.EnsureDeleted();
			_dbContext.Dispose();
		}

		[Test]
		public void GetMailBody_ValidRegisterModel_ReturnsCorrectMailBody()
		{
			// Arrange
			var registerModel = new RegisterLoginModel { UserName = "TestUser" };

			// Act
			var mailBody = _emailSenderRepository.GetMailBody(registerModel);

			// Assert
			Assert.IsNotNull(mailBody);
			Assert.IsTrue(mailBody.Contains("Confirm Email"));
			Assert.IsTrue(mailBody.Contains("Welcome to Cursus"));
		}

		[Test]
		public async Task GetUserByUsernameAsync_ValidUsername_ReturnsUser()
		{
			// Arrange
			var user = new User
			{
				UserName = "TestUser",
				Email = "testuser@example.com", // Ensure this is set
				PhoneNumber = "1234567890",     // Ensure this is set
				Password = "hashedPassword",     // Ensure this is set (hashed, if applicable)
				Status = "Pending"               // Initial status before confirmation
			};
			_dbContext.User.Add(user);
			await _dbContext.SaveChangesAsync();

			// Act
			var result = await _emailSenderRepository.GetUserByUsernameAsync("TestUser");

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual("TestUser", result.UserName);
		}

		[Test]
		public async Task EmailSendAsync_ValidEmail_ReturnsTrue()
		{
			// Arrange
			// Assuming EmailSendAsync implementation is correctly handling these
			// Also, you'll want to mock the email sending behavior in a real scenario
			// Here we are just checking if the method returns true
			var result = await _emailSenderRepository.EmailSendAsync("receiver@example.com", "Test Subject", "Test Body");

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void GenerateVerificationToken_ValidUsername_ReturnsToken()
		{
			// Act
			var token = _emailSenderRepository.GenerateVerificationToken("TestUser");

			// Assert
			Assert.IsNotNull(token);
			Assert.IsTrue(token.Length > 0);
		}

		[Test]
		public async Task ConfirmEmailAsync_ValidUsername_ReturnsSuccessMessage()
		{
			// Arrange
			var user = new User
			{
				UserName = "TestUser",
				Email = "testuser@example.com", // Ensure this is set
				PhoneNumber = "1234567890",     // Ensure this is set
				Password = "hashedPassword",     // Ensure this is set (hashed, if applicable)
				Status = "Pending"               // Initial status before confirmation
			};
			_dbContext.User.Add(user);
			await _dbContext.SaveChangesAsync();

			// Act
			var result = await _emailSenderRepository.ConfirmEmailAsync("TestUser");

			// Assert
			Assert.AreEqual("Your account has been successfully confirmed.", result);

			// Fetch the user again to check status
			var updatedUser = await _dbContext.User.FindAsync("TestUser");
			Assert.AreEqual("Verified", updatedUser.Status);
		}


		
	}
}
