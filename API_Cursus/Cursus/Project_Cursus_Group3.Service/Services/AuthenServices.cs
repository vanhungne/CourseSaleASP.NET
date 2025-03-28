using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.LoginModel;
using Project_Cursus_Group3.Data.ViewModels.LoginDTO;
using Project_Cursus_Group3.Service.Interfaces;

namespace Project_Cursus_Group3.Service.Repository
{
    public class AuthenServices : IAuthenServices
    {
        private readonly IAuthenRepository authenRepository;
        private readonly IEmailSender emailSenderRepository;

        public AuthenServices(IAuthenRepository authenRepository, IEmailSender emailSenderRepository)
        {
            this.authenRepository = authenRepository;
            this.emailSenderRepository = emailSenderRepository;
        }
        public Task<string> Login(LoginModel model)
        {
            return authenRepository.Login(model);
        }

        public Task<string> Register(RegisterLoginModel registerDTO)
        {
            return authenRepository.Register(registerDTO);
        }
        public async Task<string> ConfirmEmailAsync(string? username)
        {
            return await emailSenderRepository.ConfirmEmailAsync(username);
        }

        public string GenerateJwtToken(User user)
        {
            return authenRepository.GenerateJwtToken(user);
        }
    }
}
