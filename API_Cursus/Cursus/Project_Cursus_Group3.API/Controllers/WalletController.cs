using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.ViewModels.ReportDTO;
using Project_Cursus_Group3.Data.ViewModels.WalletDTO;
using Project_Cursus_Group3.Service.Interfaces;
using System.Security.Claims;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly IMapper mapper;
        public WalletController(IWalletService walletService, IMapper mapper)
        {
            _walletService = walletService;
            this.mapper = mapper;
        }

        [HttpGet("getwalletbyusername")]
        public async Task<IActionResult> GetWalletByUserName()
        {
            var nameIdentifierClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (nameIdentifierClaim == null)
            {
                return Unauthorized("User identity (nameidentifier) is not available in the token.");
            }

            var userName = nameIdentifierClaim.Value;

            var wallet = await _walletService.GetWalletWithTransactionsByUserNameAsync(userName);

            if (wallet == null)
            {
                return NotFound(new { message = "Wallet not found." });
            }
            var wallets = new List<Wallet> { wallet };
            return Ok(mapper.Map<List<WalletViewModel>>(wallets));
        }
    }
}
