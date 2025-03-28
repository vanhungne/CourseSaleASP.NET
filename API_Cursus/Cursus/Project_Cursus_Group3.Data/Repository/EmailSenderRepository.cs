using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Data;
using Microsoft.EntityFrameworkCore;
using Project_Cursus_Group3.Data.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Project_Cursus_Group3.Data.Model.LoginModel;
using Project_Cursus_Group3.Data.Model.UserModel;
using System.Security.Policy;

namespace Project_Cursus_Group3.Data.Repository
{
    public class EmailSenderRepository : IEmailSender
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<EmailSenderRepository> _logger;
		private readonly CursusDbContext _dbcontext;

		public EmailSenderRepository(IConfiguration configuration, CursusDbContext dbcontext, ILogger<EmailSenderRepository> logger)
		{
			_configuration = configuration;
			_logger = logger;
			_dbcontext = dbcontext;
		}
        public string GetMailBody(RegisterLoginModel registerDTO)
        {
            string apiUrl = _configuration["Host:https"];
            string token = GenerateVerificationToken(registerDTO.UserName);

            // Liên kết xác nhận sử dụng phương thức GET
            string url = $"{apiUrl}/api/Authen/confirm-email?token={token}";
            return string.Format(@"<div style='text-align:center;'>
                            <h1>Welcome to Cursus</h1>
                            <h3>Click the button below to verify your Email</h3>
                            <a href='{0}' style=' display: block;
                                                     text-align: center;
                                                     font-weight: bold;
                                                     background-color: #008CBA;
                                                     font-size: 16px;
                                                     border-radius: 10px;
                                                     color:#ffffff;
                                                     cursor:pointer;
                                                     width:100%;
                                                     padding:10px;'>
                                Confirm Email
                            </a>
                          </div>", url);
        }

        public string GetMailBodyUpdateProfile(string userName,UserProfileUpdateModel updateModel)
        {

            string apiUrl = _configuration["Host:https"];
            string token = GenerateVerificationToken(userName);

            // Liên kết xác nhận sử dụng phương thức GET
            string url = $"{apiUrl}/api/Authen/confirm-email?token={token}";
            return string.Format(@"<div style='text-align:center;'>
                            <h1>Welcome to Cursus</h1>
                            <h3>Click the button below to verify your Email</h3>
                            <a href='{0}' style=' display: block;
                                                     text-align: center;
                                                     font-weight: bold;
                                                     background-color: #008CBA;
                                                     font-size: 16px;
                                                     border-radius: 10px;
                                                     color:#ffffff;
                                                     cursor:pointer;
                                                     width:100%;
                                                     padding:10px;'>
                                Confirm Email
                            </a>
                          </div>", url);
        }


        public string GetMailBodyUpdatePassword(UpdatePasswordModel updatePassword)
        {

            string apiUrl = _configuration["Host:https"];
            //string token = GenerateVerificationToken(userName);

            // Liên kết xác nhận sử dụng phương thức GET
            //string url = $"{apiUrl}/api/Authen/update-password?token={token}";
            /*return string.Format(@"<h1>Welcome to Cursus</h1>
                                   <p>You're receiving this email because you have changed your password</p>
                                   <details>
                                    <summary>New password (click to view)</summary>
                                    {0}
                                   </details>
                                   <p>If you didn't change your password, please notify the admin via email</p>", updatePassword.NewPassword);*/

            StringBuilder mailBody = new StringBuilder();
            mailBody.AppendLine("<h1 style='color: #000000';>Welcome to Cursus</h1>");
            mailBody.AppendLine("<p style='color: #000000'>You're receiving this email because you have changed your password</p>");
            mailBody.AppendFormat("<p style='color: #000000'>New password (highlight to view): ");
            mailBody.AppendLine($"<span style='background-color: black;color: transparent'>{updatePassword.NewPassword}</span>");
            mailBody.AppendLine("</p>");
            mailBody.AppendLine("<p style='color: #000000'>If you didn't change your password, please notify the admin via email</p>");
            //Add admin contact information here
            mailBody.AppendLine("<h5 style='color: #000000'>Best regards,<br>Cursus Team</h5>");
            return mailBody.ToString();
        }

        public async Task<bool> EmailSendAsync(string email, string subject, string message)
		{
			bool status = false;
			try
			{
				var secretKey = _configuration["AppSettings:SecretKey"]; 
				var from = _configuration["AppSettings:EmailSettings:From"]; 
				var smtpServer = _configuration["AppSettings:EmailSettings:SmtpServer"];
				var port = int.Parse(_configuration["AppSettings:EmailSettings:Port"]); 
				var enableSSL = bool.Parse(_configuration["AppSettings:EmailSettings:EnablSSL"]); 
				
				var mailMessage = new MailMessage
				{
					From = new MailAddress(from),
					Subject = subject,
					Body = message,
					BodyEncoding = Encoding.UTF8,
					IsBodyHtml = true
				};
				mailMessage.To.Add(email);

				var smtpClient = new SmtpClient(smtpServer)
				{
					Port = port,
					Credentials = new NetworkCredential(from, secretKey),
					EnableSsl = enableSSL
				};

				// Gửi email
				await smtpClient.SendMailAsync(mailMessage);
				status = true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while sending the email.");
				status = false;
			}
			return status;
		}
		public async Task<User?> GetUserByUsernameAsync(string? username)
		{
			if (string.IsNullOrWhiteSpace(username))
			{
				throw new ArgumentNullException(nameof(username), "Username cannot be null or empty."); // Ném ngoại lệ nếu username không hợp lệ
			}
			return await _dbcontext.User?.FirstOrDefaultAsync(u => u.UserName == username);
		}

		public async Task UpdateUserAsync(User user)
		{
			_dbcontext.User.Update(user);
			await _dbcontext.SaveChangesAsync();
		}

        public string GenerateVerificationToken(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, username),
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(2), 
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<string> ConfirmEmailAsync(string username)
		{
			var user = await GetUserByUsernameAsync(username);
			if (user == null)
			{
				return "Invalid confirmation request.";
			}
            
            if (user.isVerify == true)
            {
                return "Your email has already been verified.";
            }

            user.isVerify = true;
			await UpdateUserAsync(user);

			return "Your account has been successfully confirmed.";
		}
	}
}
