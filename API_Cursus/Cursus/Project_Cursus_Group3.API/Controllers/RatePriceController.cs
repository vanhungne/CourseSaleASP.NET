using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Data.Model.RatePrice;
using Project_Cursus_Group3.Service.Interfaces;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]

    public class RatePriceController : ControllerBase
    {
        private readonly IRatePriceService _ratePriceService;

        public RatePriceController(IRatePriceService ratePriceService)
        {
            _ratePriceService = ratePriceService;
        }

        [HttpGet]
        public IActionResult GetCurrentRate()
        {
            var currentRate = _ratePriceService.GetCurrentRatePrice();
            return Ok(new { Rate = currentRate });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRate([FromBody] RatePrice request)
        {
            if (request.rate <= 0)
            {
                return BadRequest(new { Message = "New rate must be greater than zero." });
            }

            var result = await _ratePriceService.UpdateRatePriceAsync(request.rate);

            if (result)
            {
                return Ok(new { Message = "Rate price updated successfully" });
            }
            else
            {
                return StatusCode(500, new { Message = "An error occurred while updating the rate price" });
            }

        }
    }
}
