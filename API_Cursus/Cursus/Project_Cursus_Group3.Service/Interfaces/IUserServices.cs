using Microsoft.AspNetCore.Http;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.UserModel;
using Project_Cursus_Group3.Data.ViewModels.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface IUserServices
    {
        Task<User?> GetUserByUserName(string UserName);

        Task<User?> GetUserByUserNameByPending(string UserName);
        Task<User?> GetUserByEmailByPending(string email);
        Task<User?> GetUserByPhoneByPending(string phone);
        Task UpdateUser(User user);
        Task<User?> UpdateUsersAsync(string name, User users);

        Task<User> UpdateProfileAsync(string userName, UserProfileUpdateModel userProfileUpdateModel);
        Task<IEnumerable<UserViewGet>> GetAllUser(UserSearchOptions searchOptions, bool sortByDOB, bool ascending);

        Task<User> DeleteUser(string userName, string comment);

        Task<User> CreateUser(User user);
        Task<User> GetUserByEmail(string email);
        Task<bool> CheckBookmarkExists(string userName);
        Task<bool> CheckWalletExists(string userName);

        Task<User> UpdatePasswordAsync(string userName, UpdatePasswordModel updatePassword);
    }
}
