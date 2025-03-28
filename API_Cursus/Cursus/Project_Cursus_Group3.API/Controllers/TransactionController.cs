using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Service.Interfaces;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]

    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpDelete("/Delete-transaction/{transactionId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTransaction(int transactionId)
        {
            try
            {
                var transactionDeleted = await _transactionService.DeleteTransaction(transactionId);

                if (transactionDeleted)
                {
                    return Ok(new { message = "Transaction deleted successfully." });
                }
                else
                {
                    return NotFound(new { message = "Transaction not found or could not be deleted." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while deleting transaction with ID {transactionId}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the transaction." });
            }
        }


    }
}
