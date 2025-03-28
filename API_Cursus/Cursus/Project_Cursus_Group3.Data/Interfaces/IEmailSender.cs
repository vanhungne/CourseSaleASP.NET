using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.LoginModel;
using Project_Cursus_Group3.Data.Model.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Interfaces
{
    public interface IEmailSender
	{
		Task<bool> EmailSendAsync(string email, string subject, string message);
        string GetMailBodyUpdateProfile(string userName,UserProfileUpdateModel updateModel);

        string GetMailBody(RegisterLoginModel registerDTO);
		Task<User> GetUserByUsernameAsync(string username);
		Task UpdateUserAsync(User user);
		Task<string> ConfirmEmailAsync(string username);
        string GetMailBodyUpdatePassword(UpdatePasswordModel updatePassword);
    }
}
