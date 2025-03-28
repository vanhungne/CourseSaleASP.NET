using Microsoft.AspNetCore.Http;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.UserModel;
using Project_Cursus_Group3.Data.Repository;
using Project_Cursus_Group3.Data.ViewModels.UserDTO;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Repository
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository userRepository;

        public UserServices(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<IEnumerable<UserViewGet>> GetAllUser(UserSearchOptions searchOptions, bool sortByDOB, bool ascending)
        {
            return await userRepository.GetAllUserActiveAsync(searchOptions, sortByDOB, ascending);
        }

        public Task<User?> GetUserByEmailByPending(string email)
        {
            return userRepository.GetUserByEmailByPending(email);
        }

        public Task<User?> GetUserByPhoneByPending(string phone)
        {
            return userRepository.GetUserByPhoneByPending(phone);
        }

        public Task<User?> GetUserByUserName(string UserName)
        {
            return userRepository.GetUserByUserName(UserName);
        }

        public Task<User?> GetUserByUserNameByPending(string UserName)
        {
            return userRepository.GetUserByUserNameByPending(UserName);
        }

        public Task UpdateUser(User user)
        {
            return userRepository.UpdateUser(user);
        }

        public Task<User> UpdateProfileAsync(string userName, UserProfileUpdateModel userProfileUpdateModel)
        {
          
         return  userRepository.UpdateProfileAsync(userName, userProfileUpdateModel);

        }

        public async Task<User> DeleteUser(string userName, string comment)
        {
            return await userRepository.DeleteAsync(userName, comment);
        }

        public async Task<User> CreateUser(User user)
        {
            return await userRepository.CreateUser(user);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await userRepository.GetUserByEmail(email);
        }
        public async Task<bool> CheckBookmarkExists(string userName)
        {
            return await userRepository.CheckBookmarkExists(userName);
        }
        public async Task<bool> CheckWalletExists(string userName)
        {
            return await userRepository.CheckWalletExists(userName);
        }

        public async Task<User> UpdatePasswordAsync(string userName, UpdatePasswordModel updatePassword)
        {
            return await userRepository.UpdatePasswordAsync(userName, updatePassword);
        }
        public async Task<User?> UpdateUsersAsync(string name, User users)
        {
            return await userRepository.UpdateUsersAsync(name, users);
        }
    }
}
