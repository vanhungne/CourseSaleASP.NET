using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.LoginModel;
using Project_Cursus_Group3.Data.ViewModels.LoginDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Interfaces
{
    public interface IAuthenRepository
    {

        Task<string> Login(LoginModel model);
        Task<string> Register(RegisterLoginModel registerDTO);
        string GenerateJwtToken(User user);
        //Task<String> GetUserNameFromToken(HttpClient client);
    }
}
