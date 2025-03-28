using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Service.Interfaces;
using Project_Cursus_Group3.Data.Entities;
using System.Security.Claims;
using Flurl.Http;
using Project_Cursus_Group3.Service.Repository;
using Microsoft.AspNetCore.Cors;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]

    public class GoogleAuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserServices _userServices;
        private readonly IAuthenServices _authenServices;

        public GoogleAuthController(IConfiguration configuration, IUserServices userServices, IAuthenServices authenServices)
        {
            _configuration = configuration;
            _userServices = userServices;
            _authenServices = authenServices;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleCallback") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }


        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return BadRequest("Authentication failed.");

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

            var user = await _userServices.GetUserByEmail(email);

            if (user == null)
            {
                var encodedName = Uri.EscapeDataString(name);
                var encodedEmail = Uri.EscapeDataString(email);

                return Redirect($"https://localhost:7269/api/GoogleAuth/select-role-user?email={encodedEmail}&name={encodedName}");
            }
            string token = await GenerateJWTTOKEN(user);
            return Ok(new { code = 200, message = "Login successful", token });
        }


        [HttpPost("select-role-user")]
        public async Task<IActionResult> SelectRole([FromQuery] string email, [FromQuery] string name, [FromBody] int roleId)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Missing email or name.");
            }

            var existingUser = await _userServices.GetUserByEmail(email);
            if (existingUser != null)
            {
                return BadRequest("User already exists.");
            }
            string status = "";
            if(roleId == 1)
            {
                status = "Active";
            }else if (roleId == 2)
            {
                status = "Pending";
            }
            var user = new User
            {
                UserName = email,
                Email = email,
                FullName = name,
                Password = GenerateRandomPassword(),
                PhoneNumber = "",
                Address = "",
                CreatedDate = DateTime.UtcNow,
                DOB = DateTime.UtcNow,
                Status = status,
                RoleId = roleId,
                isVerify = true
            };

            await _userServices.CreateUser(user);


            string token = await GenerateJWTTOKEN(user);
            return Ok(new { message = "User created successfully", token });
        }
        private async Task<string> GenerateJWTTOKEN(User user)
        {

            if (user.Bookmark == null && !(await _userServices.CheckBookmarkExists(user.UserName)))
            {
                var bookmark = new Bookmark
                {
                    UserName = user.UserName,
                };
                user.Bookmark = bookmark;
                await _userServices.UpdateUser(user);
            }

            if (user.Wallet == null && !(await _userServices.CheckWalletExists(user.UserName)))
            {
                var wallet = new Wallet
                {
                    UserName = user.UserName,
                    Balance = 10000,
                    TransactionTime = DateTime.UtcNow.AddHours(7),
                };
                user.Wallet = wallet;
                await _userServices.UpdateUser(user);
            }
            //----------------------------------------------------------------------
            var token = _authenServices.GenerateJwtToken(user);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Secure = true, 
                SameSite = SameSiteMode.None, 
                Expires = DateTime.Now.AddDays(7)
            };
            Response.Cookies.Append("authToken", token, cookieOptions);
            return  token;
        }
        private string GenerateRandomPassword()
        {

            return "defaultPassword";
        }

    }


}
