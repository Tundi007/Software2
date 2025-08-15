using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StorageService.Application.Interfaces;
using StorageService.Contracts;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [VerifyUser]
    public class UserStorageController : ControllerBase
    {
        private readonly IUserStorageService _userStorageService;
        private readonly HttpClient _httpClient;

        public UserStorageController(IUserStorageService userStorageService, HttpClient httpClient)
        {
            _userStorageService = userStorageService;
            _httpClient = httpClient;
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

            var url = "https://sandbox.zarinpal.com/pg/v4/payment/request.json";

            var requestData = new
            {
                merchant_id = "15331214-463e-40c5-a428-5dc3f77d7de4",
                amount = res.StorageType.Price * 10,
                callback_url = "http://localhost:3000/payment/verify",
                description = "Transaction description.",
                metadata = new
                {
                    mobile = "09121234567",
                    email = "info.test@gmail.com"
                }
            };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Add("accept", "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();


            return Ok(new { content= result, data = res});
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

        [HttpPost("ActiveStorage")]
        public async Task<IActionResult> ActiveStorage(int userStorageId)
        {
            var res = await _userStorageService.Activate(userStorageId);
            if(res == null)
            {
                return BadRequest("sth went wrong");
            }

            return Ok(res);
        }

        [HttpPost("Public-Private")]
        public async Task<IActionResult> Public_Private(int userStorageId)
        {
            var res = await _userStorageService.TogglePublic(userStorageId);
            if (res)
            {
                return Ok("Public");
            }
            else
            {
                return Ok("Private");
            }
        } 
    }
}
