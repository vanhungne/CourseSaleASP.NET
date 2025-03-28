using Microsoft.AspNetCore.Http;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.UserModel;
using Project_Cursus_Group3.Data.ViewModels.UserDTO;

namespace Project_Cursus_Group3.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUserName(string UserName);

        Task<User?> GetUserByUserNameByPending(string UserName);
        Task<User?> GetUserByEmailByPending(string email);
        Task<User?> GetUserByPhoneByPending(string phone);
        Task UpdateUser(User user);
        Task<IEnumerable<UserViewGet>> GetAllUserActiveAsync(UserSearchOptions searchOptions, bool sortByDOB, bool ascending);
        Task<bool> ValidateEmailAsync(string email);
        Task<string?> ReadCertificateAsync(IFormFile certificateFile);
        Task<User> UpdateProfileAsync(string userName, UserProfileUpdateModel userProfileUpdateModel);
        Task<User> UpdateUsersAsync(string name, User users);

        Task<User> DeleteAsync(string userName, string comment);

        Task<User> CreateUser(User user);
        Task<User> GetUserByEmail(string email);
        Task<bool> CheckBookmarkExists(string userName);
        Task<bool> CheckWalletExists(string userName);
        Task<User> UpdatePasswordAsync(string userName, UpdatePasswordModel updatePassword);
    }
}
