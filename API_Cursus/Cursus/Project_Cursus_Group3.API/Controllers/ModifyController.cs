using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.ViewModels.StoreInforDTO;
using Project_Cursus_Group3.Service.Interfaces;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModifyController : ControllerBase
    {
        private readonly IStoreInforSerrvice _storeInforSerrvice;

        public ModifyController(IStoreInforSerrvice storeInforSerrvice)
        {
            _storeInforSerrvice = storeInforSerrvice;
        }
        [HttpGet("get-inforStore")]
        public async Task<IActionResult> Get()
        {
            var storeInfor = await _storeInforSerrvice.GetStoreViewModel();
            if (storeInfor == null)
            {
                return NotFound("Store information not found.");
            }
            return Ok(storeInfor);
        }
        [HttpPost("add-update-inforStore")]
        public async Task<IActionResult> AddOrUpdateStoreInfor([FromBody] StoreViewModel storeInfo)
        {
            if (storeInfo == null)
            {
                return BadRequest("Invalid store information.");
            }
            var result = await _storeInforSerrvice.AddOrUpdateStoreInfor(storeInfo);
            return Ok(result);
        }
        [HttpDelete("delete-storeInfor/{id}")]
        public async Task<IActionResult> DeleteStoreInfor(int id)
        {
            await _storeInforSerrvice.DeleteStoreInfor(id);
            return Ok($"Store information with id {id} has been deleted.");
        }
    }
}
