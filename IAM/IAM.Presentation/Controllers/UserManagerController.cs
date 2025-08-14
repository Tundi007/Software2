using IAM.Application.AuthenticationService.Interfaces;
using IAM.Contracts.User;
using IAM.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IAM.Presentation.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserManagerController : ControllerBase
    {
        private readonly IUserUpdateService _userUpdateService;
        private readonly IUserManageService _userManageService;

        public UserManagerController(IUserUpdateService userUpdateService, IUserManageService userManageService)
        {
            _userUpdateService = userUpdateService;
            _userManageService = userManageService;
        }


        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromBody] UserUpdateVM userUpdate)
        {
            UserUpdateVM user = await _userUpdateService.handle(userUpdate);

            if (user.UserId == -1)
            {
                return BadRequest("sth went wrong");
            }

            return Ok(user);
        }

        [HttpPost("Avatar")]
        public async Task<IActionResult> Edit([FromBody] AvatarVM avatar)
        {
            User user = await _userUpdateService.handle(avatar);

            if (user.UserId == -1)
            {
                return BadRequest("sth went wrong");
            }

            return Ok(user.Avatar);
        }


        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("amir");
        }



        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var res = _userManageService.GetUsers();
            return Ok(res);
        }

        [HttpGet("CheckUserID/{id}")]
        public async Task<IActionResult> CheckUser(int id)
        {
            var res = await _userManageService.CheckUser(id);
            if (res == null)
            {
                return NotFound("User Does not exist");
            }
            else
            {
                return Ok();
            }
        }
    }
}
