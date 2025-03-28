using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Data.Model.VNPAY;
using Project_Cursus_Group3.Service.Interfaces;
using System.Security.Claims;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayPalController : ControllerBase
    {
        private readonly IPayPalService _payPalService;
        private readonly ILogger<PayPalController> _logger;

        public PayPalController(IPayPalService payPalService, ILogger<PayPalController> logger)
        {
            _payPalService = payPalService;
            _logger = logger;
        }

        [HttpPost("create-Payment")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentInformationModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidOperationException("Invalid data");
            }
            //try
            //{
            var url = await _payPalService.CreatePaymentAsync(model, HttpContext);
            return Ok(new { url });
            //}
            //catch (Exception ex) {
            //    _logger.LogError(ex, "Error creating PayPal payment");
            //    return StatusCode(500, "An error occurred while processing your request");
            //}
        }
        [HttpGet("capture-payment")]
        public async Task<IActionResult> CapturePayment([FromQuery] string token, [FromQuery] int? orderId,
        [FromQuery] string username, [FromQuery] string orderType, [FromQuery] decimal amount)
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Missing token in capture payment request");
                return BadRequest("Token is required.");
            }
            if (orderType != "wallet" && !orderId.HasValue)
            {
                _logger.LogWarning("Missing orderId for non-wallet payment type");
                return BadRequest("OrderId is required for this payment type.");
            }
            try
            {
                var model = new PaymentInformationPaypal
                {
                    OrderId = orderId ?? 0,
                    OrderType = orderType,
                    Amount = amount,
                };
                var captureResult = await _payPalService.CapturePaymentAsync(token);
                if (captureResult.Status == "COMPLETED")
                {
                    var transactionId = captureResult.OrderId;
                    var processResult = _payPalService.ProcessPaymentCompleteAsync(model, username, transactionId);
                    return Ok(new { message = "Payment successfully captured and processed.", result = processResult });
                }
                else
                {
                    _logger.LogWarning("Payment capture was not completed. Status: {Status}", captureResult.Status);
                    return BadRequest("Payment capture failed or is incomplete.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing PayPal payment");
                return StatusCode(500, "An error occurred while processing the payment.");
            }
        }
        [HttpGet("admin-process-refund")]
        public async Task<IActionResult> isApprove(AdminRefundApprovalModel model)
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userName == null) {
                return BadRequest("null");

            }
            var isApprove = await _payPalService.ApproveRefundAsync(model,userName);

            return Ok(isApprove);
        }
        
            
       

        [HttpPost("create-refund")]
        public async Task<IActionResult> RefundOrder([FromBody] RefundRequestModel model)
        {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _payPalService.RequestRefundAsync(model);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
    }
}
