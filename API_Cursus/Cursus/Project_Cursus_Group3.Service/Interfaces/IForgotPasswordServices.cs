using Project_Cursus_Group3.Data.ViewModels;
using Project_Cursus_Group3.Data.ViewModels.ForgotDTO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface IForgotPasswordServices
    {
        Task<string> RequestPasswordReset(PasswordResetRequest model);
        Task SendResetEmail(string email, string resetLink);
        string GenerateResetToken(string email);
        ClaimsPrincipal GetPrincipalFromToken(string token);
        Task UpdatePasswordAsync(string email, string newPassword);
    }
}
