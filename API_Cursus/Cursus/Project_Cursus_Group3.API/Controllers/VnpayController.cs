using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Data.Model.VNPAY;
using Project_Cursus_Group3.Service.Interfaces;
using System.Security.Claims;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]

    [Authorize]
    public class VnpayController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;

        public VnpayController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost("payment")]
        public IActionResult TopUpWallet([FromBody] PaymentInformationModel model)
        {
            //var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (model.Name != currentUserId)
            //{
            //    return StatusCode(403, "You cannot add money to someone else's wallet.");
            //}

            var url = _vnPayService.CreatePaymentUrlAsync(model, HttpContext);
            return Ok(new { PaymentUrl = url });
        }

        [HttpGet("payment-callback")]
        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            return Ok(response);
        }
    }
}
