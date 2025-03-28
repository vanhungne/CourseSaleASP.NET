using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.LoginModel;
using Project_Cursus_Group3.Data.ViewModels.LoginDTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Project_Cursus_Group3.Data.Repository
{
    public class AuthenRepository : IAuthenRepository
    {
        private readonly IConfiguration _configuration;
        private readonly CursusDbContext _dbcontext;
        private readonly IEmailSender emailSender;

        public AuthenRepository(IConfiguration configuration, CursusDbContext dbcontext, IEmailSender emailSender)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            this.emailSender = emailSender;
        }

        public async Task<string> Login(LoginModel model)
        {
            var user = await _dbcontext.User.Include(u => u.Role).AsNoTracking()
                                             .Where(x => x.Status == "Active")
                                             .SingleOrDefaultAsync(u => u.Email.Equals(model.Email));

            if (user == null)
            {
                return null;
            }

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return null;
            }
            var claims = new[]
                {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.NameIdentifier, user.UserName.ToString()),
            new Claim(ClaimTypes.Role, user.Role.RoleName),
            new Claim("RoleId", user.RoleId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("Avatar", user.Avatar.ToString()),
               };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }




        public async Task<string> Register(RegisterLoginModel registerDTO)
        {
            try
            {
                var existingEmail = await _dbcontext.User.FirstOrDefaultAsync(u => u.Email == registerDTO.Email);
                if (existingEmail != null)
                {
                    return "Email already exists";
                }

                var existingUser = await _dbcontext.User.FirstOrDefaultAsync(u => u.UserName == registerDTO.UserName);
                if (existingUser != null)
                {
                    return "UserName already exists";
                }
                var existingPhone = await _dbcontext.User.FirstOrDefaultAsync(u => u.PhoneNumber == registerDTO.PhoneNumber);
                if (existingPhone != null)
                {
                    return "Phone number already exists";
                }

                if (registerDTO.RoleId == 1)
                {
                    registerDTO.Status = "Active";

                    string emailBody = emailSender.GetMailBody(registerDTO);
                    bool emailSent = await emailSender.EmailSendAsync(registerDTO.Email, "Account Created", emailBody);

                    if (!emailSent)
                    {
                        return "There was an error sending the email. Please try again later.";
                    }
                }
                else if (registerDTO.RoleId == 2)
                {
                    registerDTO.Status = "Pending";
                }

                var newUser = new User
                {
                    UserName = registerDTO.UserName,
                    FullName = registerDTO.FullName,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password),
                    Address = registerDTO.Address,
                    PhoneNumber = registerDTO.PhoneNumber,
                    Email = registerDTO.Email,
                    DOB = registerDTO.DOB,
                    RoleId = registerDTO.RoleId,
                    CreatedDate = registerDTO.CreatedDate,
                    Status = registerDTO.Status
                };
                //if (registerDTO.Certification != null)
                //{
                //    var certificationUrl = await UploadFileAsync(registerDTO.Certification, "certification");
                //    newUser.Certification = certificationUrl;
                //}
                if (registerDTO.Avatar != null)
                {
                    var avatarUrl = await UploadFileAsync(registerDTO.Avatar, "avatar");
                    newUser.Avatar = avatarUrl;
                }

                if (registerDTO.RoleId == 1)
                {
                    Bookmark bookmark = new Bookmark
                    {
                        UserName = newUser.UserName,
                    };
                    newUser.Bookmark = bookmark;
                }


                Wallet wallet = new Wallet
                {
                    UserName = newUser.UserName,
                    Balance = 10000,
                    TransactionTime = DateTime.UtcNow.AddHours(7),
                };
                newUser.Wallet = wallet;


                _dbcontext.User.Add(newUser);
                await _dbcontext.SaveChangesAsync();

                if (registerDTO.RoleId == 1)
                {
                    return "Pls check email to verify.";
                }
                else if (registerDTO.RoleId == 2)
                {
                    return "Your account is pending admin approval.";
                }

                return "User registered successfully.";
            }
            catch (Exception ex)
            {
                return $"Internal server error: {ex.Message}";
            }
        }



        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                var stream = file.OpenReadStream();
                var bucket = _configuration["FireBase:Bucket"];

                var task = new FirebaseStorage(bucket)
                    .Child("certifications")
                    .Child(file.FileName)
                    .PutAsync(stream);

                var downloadUrl = await task;

                return downloadUrl;
            }
            return null;
        }



        public string GenerateJwtToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.UserName?.ToString() ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role?.RoleName ?? string.Empty),
            new Claim("RoleId", user.RoleId.ToString() ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim("Avatar", user.Avatar?.ToString() ?? string.Empty),
            };



            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //khhds
        public async Task<string> UploadFileAsync(IFormFile file, string fileType)
        {
            if (file.Length > 0)
            {
                var stream = file.OpenReadStream();
                var bucket = _configuration["FireBase:Bucket"];

                string folderName = fileType == "certification" ? "certifications" : "avatars";

                var task = new FirebaseStorage(bucket)
                    .Child(folderName)
                    .Child(file.FileName)
                    .PutAsync(stream);

                var downloadUrl = await task;

                return downloadUrl;
            }
            return null;
        }




    }
}
