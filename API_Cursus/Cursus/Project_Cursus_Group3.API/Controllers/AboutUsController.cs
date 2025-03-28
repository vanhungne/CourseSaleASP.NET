using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.AboutUsModel;
using Project_Cursus_Group3.Data.ViewModels.CourseDTO;
using Project_Cursus_Group3.Data.ViewModels.PagingDTO;

namespace Project_Cursus_Group3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutUsController : ControllerBase
    {
        private readonly IAboutUsRepository _aboutUsRepository;

        public AboutUsController(IAboutUsRepository aboutUsRepository)
        {
            _aboutUsRepository = aboutUsRepository;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 20)
        {
            var aboutUsList = await _aboutUsRepository.GetAll();
            var Paging = aboutUsList.ToPagedList(page, pageSize);
            var response = new PagingModel<AboutUs>
            {
                CurrentPage = page,
                TotalPages = Paging.PageCount,
                PageSize = pageSize,
                TotalCount = Paging.TotalItemCount,
                Items = Paging.ToList()
            };
            return Ok(response);
        }
        [HttpGet("get-aboutUs/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var aboutUs = await _aboutUsRepository.Get(id);
            if (aboutUs == null)
            {
                return NotFound("Not found");
            }
            return Ok(aboutUs);
        }
        [HttpPost("add-aboutUs")]
        public async Task<IActionResult> Add([FromBody] CreateAboutUs aboutUs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newAboutUs = await _aboutUsRepository.Add(aboutUs);
            return CreatedAtAction(nameof(Get), new { id = newAboutUs.Id }, newAboutUs);
        }
        [HttpPut("update-aboutUs/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateAboutUs updateAboutUs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedAboutUs = await _aboutUsRepository.Update(id, updateAboutUs);
            if (updatedAboutUs == null)
            {
                return NotFound("About Us not found");
            }
            return Ok("Update Successfull");
        }
        [HttpDelete("delete-aboutUs/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var aboutUs = await _aboutUsRepository.Get(id);
            if (aboutUs == null)
            {
                return NotFound("About Us not found");
            }

            await _aboutUsRepository.Delete(id);
            return Ok("Delete successfully");
        }
    }
}
