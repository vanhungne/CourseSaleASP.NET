using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Project_Cursus_Group3.Service.Interfaces;
using Project_Cursus_Group3.Data.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Project_Cursus_Group3.Data.ViewModels.ForgotDTO;

namespace Project_Cursus_Group3.Service.Services
{
    public class ForgotPasswordServices : IForgotPasswordServices
    {
        private readonly JwtSettings _jwtSettings;
        private readonly SmtpResetPassword _smtpSettings;
        private readonly IUserServices _userServices;

        public ForgotPasswordServices(IOptions<JwtSettings> jwtSettings, IOptions<SmtpResetPassword> smtpSettings, IUserServices userServices)
        {
            _jwtSettings = jwtSettings.Value;
            _smtpSettings = smtpSettings.Value;
            _userServices = userServices;
        }

        public async Task<string> RequestPasswordReset(PasswordResetRequest model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                throw new ArgumentException("Email address is required.");
            }

            var user = await _userServices.GetUserByEmail(model.Email);
            if (user == null)
            {
                throw new ArgumentException("Invalid email address.");
            }

            var token = GenerateResetToken(user.Email);
            var resetLink = $"http://CursusManagement/reset-password?token={token}";
            await SendResetEmail(model.Email, resetLink);

            return "Password reset link has been sent to your email.";
        }

        public string GenerateResetToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email) }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task SendResetEmail(string email, string resetLink)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username),
                Subject = "Password Reset",
                IsBodyHtml = true
            };

            var bodyBuilder = new StringBuilder();
            bodyBuilder.AppendLine("<html><body style='font-family: Arial, sans-serif; color: #333;'>");
            bodyBuilder.AppendLine("<div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eaeaea; border-radius: 5px; background-color: #f9f9f9;'>");
            bodyBuilder.AppendLine("<h2 style='color: #007BFF;'>Password Reset Request</h2>");
            bodyBuilder.AppendLine("<p>Hi there,</p>");
            bodyBuilder.AppendLine("<p>You requested a password reset. Please click the link below to reset your password:</p>");
            bodyBuilder.AppendLine($"<p><a href=\"{resetLink}\" style=\"background-color: #007BFF; color: white; padding: 10px 15px; text-decoration: none; border-radius: 5px;\">Reset Password</a></p>");
            bodyBuilder.AppendLine("<p>If you did not request this, please ignore this email.</p>");
            bodyBuilder.AppendLine("<p>Best regards,<br>Your Team</p>");
            bodyBuilder.AppendLine("</div>");
            bodyBuilder.AppendLine("</body></html>");

            mailMessage.Body = bodyBuilder.ToString();
            mailMessage.To.Add(email);

            using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
            };

            await smtpClient.SendMailAsync(mailMessage);
        }


        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return validatedToken is JwtSecurityToken ? principal : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task UpdatePasswordAsync(string email, string newPassword)
        {
            var user = await _userServices.GetUserByEmail(email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.Password = newPassword;

            await _userServices.UpdateUsersAsync(user.UserName, user);
        }
    }
}
