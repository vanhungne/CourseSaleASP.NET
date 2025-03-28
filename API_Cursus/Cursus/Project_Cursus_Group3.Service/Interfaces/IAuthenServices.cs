using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.LoginModel;
using Project_Cursus_Group3.Data.ViewModels.LoginDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface IAuthenServices
    {
        Task<string> Login(LoginModel model);
        Task<string> Register(RegisterLoginModel registerDTO);
		Task<string> ConfirmEmailAsync(string? username);
        string GenerateJwtToken(User user);
        //Task<string> GetUsername
    }
}
