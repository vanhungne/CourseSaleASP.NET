using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Data.Model.CertificationModel;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificationController : ControllerBase
    {
        private readonly ICertificationService _service;

        public CertificationController(ICertificationService service)
        {
            _service = service;
        }

        private string GetUsername()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private IActionResult ValidateUserAndId(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid certification ID.");

            var userName = GetUsername();
            if (string.IsNullOrEmpty(userName))
                return Unauthorized("User not authenticated.");

            return null;
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var validationResult = ValidateUserAndId(id);
            if (validationResult != null) return validationResult;

            try
            {
                var userName = GetUsername();
                var certification = await _service.GetByIdAndUsernameAsync(id, userName);
                if (certification == null)
                    return NotFound($"Certification with ID {id} not found for user {userName}.");

                return Ok(certification);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("update-id/{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] UpdateCertification model)
        {
            var validationResult = ValidateUserAndId(id);
            if (validationResult != null) return validationResult;

            if (model == null)
                return BadRequest("Invalid model data.");

            try
            {
                var userName = GetUsername();
                var updatedCertification = await _service.UpdateAsync(id, model, userName);
                if (updatedCertification == null)
                    return NotFound($"Certification with ID {id} not found or not accessible for user {userName}.");

                return Ok(new { Message = "Update successful", Certification = updatedCertification });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var validationResult = ValidateUserAndId(id);
            if (validationResult != null) return validationResult;

            try
            {
                var userName = GetUsername();
                var certification = await _service.GetByIdAndUsernameAsync(id, userName);
                if (certification == null)
                    return NotFound($"Certification with ID {id} not found or not accessible for user {userName}.");

                await _service.DeleteAsync(id);
                return Ok(new { Message = "Delete successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("add-certification")]
        public async Task<IActionResult> Add([FromForm] UpdateCertification model)
        {
            if (model == null || model.Certification == null)
                return BadRequest("Certification model or file cannot be null.");

            var userName = GetUsername();
            if (string.IsNullOrEmpty(userName))
                return Unauthorized("User not authenticated.");

            try
            {
                var newCertification = await _service.AddAsync(model, userName);
                if (newCertification == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to add certification.");

                return Ok(new { Message = "Certification added successfully", Certification = newCertification });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
