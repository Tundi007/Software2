using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StorageService.Application.Interfaces;
using StorageService.Contracts;

namespace StorageService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageTypeController : ControllerBase
    {

        private readonly IStorageTypeService _storageTypeService;
        public StorageTypeController(IStorageTypeService storageTypeService)
        {
            _storageTypeService = storageTypeService;
        }


        [HttpGet("Get")]
        public IActionResult Get()
        {
            var res = _storageTypeService.Get();
            return Ok(res);
        }

        [HttpGet("find")]
        public async Task<IActionResult> Find(int id)
        {
            var res = await _storageTypeService.Find(id);
            if (res == null)
            {
                return NotFound("not found");
            }

            return Ok(res);
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody]StorageTypeVMCreate storageTypeVMCreate)
        {
            var res = await _storageTypeService.Create(storageTypeVMCreate);
            if(res.StorageTypeID == -1)
            {
                return BadRequest("sth went wrong");
            }

            return Ok(res);
        }


        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody]StorageTypeVM storageTypeVM)
        {
            var res = await _storageTypeService.Update(storageTypeVM);
            if (res.StorageTypeID == -1)
            {
                return BadRequest("sth went wrong");
            }

            return Ok(res);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _storageTypeService.Delete(id);

            return Ok();
        }
    }
}
