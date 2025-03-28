using AutoMapper;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.UserModel;
using Project_Cursus_Group3.Data.ViewModels.UserDTO;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly CursusDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender emailSender;

        public UserRepository(CursusDbContext context,IMapper mapper, IConfiguration configuration, IEmailSender emailSender) : base(context)
        {
            _dbContext = context;
            _mapper = mapper;
            _configuration = configuration;
            this.emailSender = emailSender;
        }

        public async Task<User?> GetUserByUserNameByPending(string UserName)
        {
            return await Entities.FirstOrDefaultAsync(x => x.UserName == UserName && x.Status.ToLower() == "pending");

        }

        public async Task<User> GetUserByEmailByPending(string email)
        {
            return await Entities.FirstOrDefaultAsync(u => u.Email == email && u.Status.ToLower() == "pending");
        }

        public async Task UpdateUser(User user)
        {
            _dbContext.User.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User?> GetUserByPhoneByPending(string phone)
        {
            return await Entities.FirstOrDefaultAsync(u => u.PhoneNumber == phone && u.Status.ToLower() == "pending");
        }

        public async Task<User?> GetUserByUserName(string userName)
        {
            return await Entities.FirstOrDefaultAsync(x => x.UserName == userName &&
                                                           (x.Status.ToLower() == "active"));
        }


        public async Task<IEnumerable<UserViewGet>> GetAllUserActiveAsync(UserSearchOptions searchOptions, bool sortByDOB, bool ascending)
        {
            var query = Entities.AsNoTracking().Include(u => u.Role).Include(u => u.Wallet).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchOptions.UserName))
            {
                query = query.Where(c => c.UserName.Contains(searchOptions.UserName));
            }

            if (!string.IsNullOrWhiteSpace(searchOptions.PhoneNumber))
            {
                query = query.Where(c => c.PhoneNumber.Contains(searchOptions.PhoneNumber));
            }



            if (sortByDOB)
            {
                query = ascending ? query.OrderBy(u => u.DOB) : query.OrderByDescending(u => u.DOB);
            }
            else
            {
                query = ascending ? query.OrderBy(u => u.CreatedDate) : query.OrderByDescending(u => u.CreatedDate);
            }

            var activeUsers = await query.Select(u => _mapper.Map<UserViewGet>(u)).ToListAsync();

            return activeUsers;

        }

        public Task<bool> ValidateEmailAsync(string email)
        {
            return Task.FromResult(Regex.IsMatch(email, @"^[\w-\.]+@(gmail\.com|fpt\.edu\.vn)$"));
        }

        public async Task<string?> ReadCertificateAsync(IFormFile certificateFile)
        {
            using (var reader = new StreamReader(certificateFile.OpenReadStream()))
            {
                return await reader.ReadToEndAsync();
            }
        }
        public async Task<User> UpdateProfileAsync(string userName, UserProfileUpdateModel userProfileUpdateModel)
        {
            var existingUser = await GetUserByUserName(userName); 

            if (existingUser == null)
            {
                throw new ArgumentException($"User {userName} not found.");
            }

            if (!string.IsNullOrEmpty(userProfileUpdateModel.Email) && userProfileUpdateModel.Email != existingUser.Email)
            {
                var duplicateEmailUser = await _dbContext.User
                    .FirstOrDefaultAsync(u => u.Email == userProfileUpdateModel.Email && u.UserName != existingUser.UserName);
                if (duplicateEmailUser != null)
                {
                    throw new ArgumentException($"Email {userProfileUpdateModel.Email} already exists.");
                }

                string emailBody = emailSender.GetMailBodyUpdateProfile(userName, userProfileUpdateModel);
                bool emailSent = await emailSender.EmailSendAsync(userProfileUpdateModel.Email, "Verify Your Email", emailBody);

                if (!emailSent)
                {
                    throw new InvalidOperationException("Error sending email.");
                }

                existingUser.Status = "Active";
                existingUser.Email = userProfileUpdateModel.Email; 
            }

            existingUser.FullName = userProfileUpdateModel.FullName ?? existingUser.FullName;
            existingUser.Address = userProfileUpdateModel.Address ?? existingUser.Address;

            if (userProfileUpdateModel.DOB != null)
            {
                existingUser.DOB = userProfileUpdateModel.DOB;
            }

            //if (userProfileUpdateModel.Certification != null)
            //{
            //    var certificationUrl = await UploadFileAsync(userProfileUpdateModel.Certification, "certification");
            //    if (certificationUrl != null)
            //    {
            //        existingUser.Certification = certificationUrl;
            //    }
            //}

            if (userProfileUpdateModel.Avatar != null)
            {
                var AvatarUrl = await UploadFileAsync(userProfileUpdateModel.Avatar, "avatar");
                if (AvatarUrl != null)
                {
                    existingUser.Avatar = AvatarUrl;
                }
            }
            _dbContext.User.Update(existingUser);
            await _dbContext.SaveChangesAsync();
            return existingUser;
        }

   

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

        public async Task<User> DeleteAsync(string userName, string comment)
        {
            var user = await GetUserByUserName(userName);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with username {userName} not found.");
            }

            user.Status = "Inactive";
            user.deleteComment = comment;
            await _dbContext.SaveChangesAsync();
            return user;
        }
        public async Task<User> CreateUser(User user)
        {
            await _dbContext.User.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await Entities.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> UpdatePasswordAsync(string userName, UpdatePasswordModel updatePassword)
        {
            var existingUser = await GetUserByUserName(userName);

            if (existingUser == null)
            {
                throw new ArgumentException($"User {userName} not found.");
            }

            if (updatePassword.NewPassword != updatePassword.ConfirmPassword)
            {
                throw new ValidationException("New password and confirm password do not match.");
            }

            if (!BCrypt.Net.BCrypt.Verify(updatePassword.OldPassword, existingUser.Password))
            {
                throw new ValidationException("Old password isn't correct.");
            }

            string emailBody = emailSender.GetMailBodyUpdatePassword(updatePassword);
            bool emailSent = await emailSender.EmailSendAsync(existingUser.Email, "Confirm Password Change", emailBody);
            if (!emailSent)
            {
                throw new ValidationException("There was an error sending the email. Please try again later.");
            }

            if (!emailSent)
            {
                throw new InvalidOperationException("Error sending email.");
            }

            existingUser.Password = BCrypt.Net.BCrypt.HashPassword(updatePassword.NewPassword);

            _dbContext.User.Update(existingUser);
            await _dbContext.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> CheckBookmarkExists(string userName)
        {

            return await _dbContext.Bookmark.AnyAsync(b => b.UserName == userName);
        }

        public async Task<bool> CheckWalletExists(string userName)
        {
            return await _dbContext.Wallet.AnyAsync(w => w.UserName == userName);
        }

        public async Task<User> UpdateUsersAsync(string name, User users)
        {
            var Checkexist = await _dbContext.User.FirstOrDefaultAsync(x => x.UserName == name);
            if (Checkexist == null) return null;
            Checkexist.FullName = users.FullName;
            Checkexist.Password = BCrypt.Net.BCrypt.HashPassword(users.Password);
            Checkexist.Address = users.Address;
            Checkexist.PhoneNumber = users.PhoneNumber;
            Checkexist.Email = users.Email;
            Checkexist.DOB = users.DOB;
            Checkexist.RoleId = users.RoleId;
            Checkexist.Avatar = users.Avatar;
            //Checkexist.Certification = users.Certification;
            //Checkexist.Comment = users.Comment;
            Checkexist.Status = users.Status;

            await _dbContext.SaveChangesAsync();
            return Checkexist;
        }
    }
}
