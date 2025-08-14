using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StorageService.Application.Interfaces;
using StorageService.Contracts;
using System.Threading.Tasks;

namespace StorageService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [VerifyUser]
    public class UserStorageController : ControllerBase
    {
        private readonly IUserStorageService _userStorageService;

        public UserStorageController(IUserStorageService userStorageService)
        {
            _userStorageService = userStorageService;
        }


        [HttpGet("GetUserStorages")]
        public IActionResult GetUserStorages(int? userId)
        {
            var res = _userStorageService.GetUserStorages(userId);

            if(res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        [HttpGet("GetUserStorage")]
        public async Task<IActionResult> GetUserStorage(int userStorageId)
        {
            var res =  await _userStorageService.Find(userStorageId);

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }


        [HttpPost("Buy")]
        public async Task<IActionResult> Buy(UserStorageCreateVM userStorageCreateVM)
        {
            var res = await _userStorageService.Buy(userStorageCreateVM);
            if (res.UserID == -1)
            {
                return BadRequest();
            }
            else if (res.UserID == -2)
            {
                return BadRequest("User Not Found");
            }

            return Ok(res);
        }

        [HttpPost("extendtime")]
        public async Task<IActionResult> ExtendTime(ExtendTimeVM extendTimeVM)
        {
            var res = await _userStorageService.ExtendTime(extendTimeVM);

            if(res.UserID == -1)
            {
                return BadRequest("User not found");
            }
            else if (res.UserID == -2)
            {
                return BadRequest("User doesnt have storage");
            }
            else if (res.UserID == -3)
            {
                return BadRequest("sth went wrong");
            }

            return Ok(res);
        }
    }
}
