using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Service.Interfaces;
using Project_Cursus_Group3.Data.ViewModels;
using System.Threading.Tasks;
using Project_Cursus_Group3.Data.ViewModels.ForgotDTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Cors;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]

    public class PasswordResetController : ControllerBase
    {
        private readonly IForgotPasswordServices _forgotPasswordServices;

        public PasswordResetController(IForgotPasswordServices forgotPasswordServices)
        {
            _forgotPasswordServices = forgotPasswordServices;
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest model)
        {
            try
            {
                var message = await _forgotPasswordServices.RequestPasswordReset(model);
                return Ok(new { Message = message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetModel model)
        {
            var principal = _forgotPasswordServices.GetPrincipalFromToken(model.Token);
            if (principal == null)
            {
                return BadRequest("Token expired please try again");
            }

            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest("Invalid token.");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            await _forgotPasswordServices.UpdatePasswordAsync(email, model.NewPassword);
            return Ok("Password has been reset.");
        }
    }
}
