using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Service.Interfaces;
using Project_Cursus_Group3.Data.Model.CartModel;
using System.Security.Claims;
using Project_Cursus_Group3.Data.Entities;
using Microsoft.AspNetCore.Cors;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    [AllowAnonymous]
    [EnableCors("AllowSpecificOrigins")]

    public class CartController : ControllerBase
    {
        private readonly ICartServices _cartServices;

        public CartController(ICartServices cartServices)
        {
            _cartServices = cartServices;
        }


        [HttpPost("Add-Cart")]
        public async Task<IActionResult> AddToCart(int courseId)
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                if (HttpContext.Session.GetString("GuestId") == null)
                {
                    var guestId = "Guest_" + Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("GuestId", guestId);
                    userName = guestId;
                }
                else
                {
                    userName = HttpContext.Session.GetString("GuestId");
                }
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Instructor" || userRole == "Admin")
            {
                return StatusCode(403, "You do not have permission to add items to the cart.");
            }

            try
            {
                var result = await _cartServices.AddToCartAsync(userName, courseId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("Get-Cart")]
        public IActionResult GetCart()
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                userName = HttpContext.Session.GetString("GuestId");

                if (string.IsNullOrEmpty(userName))
                {
                    return BadRequest("User ID or Guest ID is required.");
                }
            }

            var cartSummary = _cartServices.GetCart(userName); 
            return Ok(cartSummary); 
        }



        [HttpDelete("clear")]
        public IActionResult ClearAllCart()
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                userName = HttpContext.Session.GetString("GuestId");

                if (string.IsNullOrEmpty(userName))
                {
                    return BadRequest("User ID or Guest ID is required.");
                }
            }

            _cartServices.ClearAllCart(userName);
            return Ok("Cart cleared successfully.");
        }



        [HttpDelete("remove/{courseId}")]
        public IActionResult RemoveItemsInCart(int courseId)
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                userName = HttpContext.Session.GetString("GuestId");

                if (string.IsNullOrEmpty(userName))
                {
                    return BadRequest("User ID or Guest ID is required.");
                }
            }

            try
            {
                _cartServices.RemoveItemsInCart(userName, courseId);
                return Ok("Item removed from cart successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("You must be logged in to checkout.");
            }

            try
            {
                var result = await _cartServices.CheckoutAsync(userName);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while processing your checkout. Please try again later.");
            }
        }


    }
}
