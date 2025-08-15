using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StorageService.Application.Interfaces;
using StorageService.Contracts;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace StorageService.Presentation.Controllers
{
    [ApiController]
    [Route("api/storage")]
    [VerifyUser]
    public class StorageController : ControllerBase
    {
        private readonly ICloudStorageService _storage;
        private readonly HttpClient _httpClient;
        private readonly IUserStorageService _userStorage;
        private readonly string url;

        public StorageController(ICloudStorageService storage, HttpClient httpClient, IUserStorageService userStorageService)
        {
            _storage = storage;
            _httpClient = httpClient;
            _userStorage = userStorageService;
            url = "http://iam:8080/api/auth/CheckTokenWithUser";
        }



        [HttpGet("download")]
        public async Task<IActionResult> Download(string fileName, string userStorageId)
        {
            var us = await _userStorage.Find(int.Parse(userStorageId));

            if(us == null)
            {
                return BadRequest("Storage not Buyed or isnt active yet");
            }

            if(!us.IsPublic)
            {
                var token = Request.Headers["Authorization"].ToString();


                token = token.Substring("Bearer ".Length).Trim();

                UserStorageVM vm = new UserStorageVM();
                // create http content to send
                // send request using post
                var json = JsonSerializer.Serialize(new { token = token, UserId = us.UserID });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return Unauthorized();
                }
            }



            var stream = await _storage.DownloadAsync(fileName, container: "", userStorageId);
            return File(stream, "application/octet-stream", fileName);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string fileName, string userStorageId)
        {
            var us = await _userStorage.Find(int.Parse(userStorageId));

            if (us == null)
            {
                return BadRequest("Storage not Buyed or isnt active yet");
            }

            var token = Request.Headers["Authorization"].ToString();


            token = token.Substring("Bearer ".Length).Trim();

            UserStorageVM vm = new UserStorageVM();
            // create http content to send
            // send request using post
            var json = JsonSerializer.Serialize(new { token = token, UserId = us.UserID });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized();
            }


            var result = await _storage.DeleteAsync(fileName, container: "", userStorageId);
            return result ? Ok() : NotFound();
        }

        [HttpGet("list")]
        public async Task<IActionResult> List([FromQuery] string userStorageId, string? folder)
        {
            var us = await _userStorage.Find(int.Parse(userStorageId));

            if (us == null)
            {
                return BadRequest("Storage not Buyed or isnt active yet");
            }

            if (!us.IsPublic)
            {
                var token = Request.Headers["Authorization"].ToString();


                token = token.Substring("Bearer ".Length).Trim();

                UserStorageVM vm = new UserStorageVM();
                // create http content to send
                // send request using post
                var json = JsonSerializer.Serialize(new { token = token, UserId = us.UserID });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return Unauthorized();
                }
            }

            await _storage.EnsureUserBucketExistsAsync(userStorageId);
            var files = await _storage.ListAsync(folder, userStorageId);
            return Ok(files);
        }


        [HttpGet("usage")]
        public async Task<IActionResult> GetBucketUsage([FromQuery] string userStorageId = null)
        {
            var us = await _userStorage.Find(int.Parse(userStorageId));
            if (us == null)
            {
                return BadRequest("Storage not Buyed or isnt active yet");
            }

            if (!us.IsPublic)
            {
                var token = Request.Headers["Authorization"].ToString();


                token = token.Substring("Bearer ".Length).Trim();

                UserStorageVM vm = new UserStorageVM();
                // create http content to send
                // send request using post
                var json = JsonSerializer.Serialize(new { token = token, UserId = us.UserID });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return Unauthorized();
                }
            }


                var sizeInBytes = await _storage.GetBucketSizeAsync(userStorageId);
            var sizeInMB = Math.Round(sizeInBytes / 1024.0 / 1024.0, 2);

            return Ok(new
            {
                userStorageId = userStorageId ?? "all",
                SizeInBytes = sizeInBytes,
                SizeInMB = sizeInMB
            });
        }

        [HttpPost("create-folder")]
        public async Task<IActionResult> CreateFolder([FromQuery] string folderName, [FromQuery] string userStorageId)
        {
            var us = await _userStorage.Find(int.Parse(userStorageId));
            if (us == null)
            {
                return BadRequest("Storage not Buyed or isnt active yet");
            }

            var token = Request.Headers["Authorization"].ToString();


            token = token.Substring("Bearer ".Length).Trim();

            UserStorageVM vm = new UserStorageVM();
            // create http content to send
            // send request using post
            var json = JsonSerializer.Serialize(new { token = token, UserId = us.UserID });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized();
            }


            if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(userStorageId))
                return BadRequest("Folder name and user ID are required.");

            await _storage.CreateFolderAsync(folderName, userStorageId);
            return Ok(new { Message = $"Folder '{folderName}' created for user {userStorageId}" });
        }

        [HttpGet("folders")]
        public async Task<IActionResult> ListFolders([FromQuery] string userStorageId)
        {
            var us = await _userStorage.Find(int.Parse(userStorageId));
            if (us == null)
            {
                return BadRequest("Storage not Buyed or isnt active yet");
            }

            if (!us.IsPublic)
            {
                var token = Request.Headers["Authorization"].ToString();


                token = token.Substring("Bearer ".Length).Trim();

                UserStorageVM vm = new UserStorageVM();
                // create http content to send
                // send request using post
                var json = JsonSerializer.Serialize(new { token = token, UserId = us.UserID });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return Unauthorized();
                }
            }


            var folders = await _storage.ListFoldersAsync(userStorageId);
            return Ok(folders);
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")] // makes Swagger send multipart
        [RequestSizeLimit(5L * 1024 * 1024 * 1024)] // per-endpoint overall request size
        [RequestFormLimits(MultipartBodyLengthLimit = 5L * 1024 * 1024 * 1024)]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string userStorageId, [FromQuery] string? folder = "")
        {
            var us = await _userStorage.Find(int.Parse(userStorageId));
            if (us == null)
            {
                return BadRequest("Storage not Buyed or isnt active yet");
            }

            var token = Request.Headers["Authorization"].ToString();


            token = token.Substring("Bearer ".Length).Trim();

            UserStorageVM vm = new UserStorageVM();
            // create http content to send
            // send request using post
            var json = JsonSerializer.Serialize(new { token = token, UserId = us.UserID });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized();
            }


            try
            {
                await _storage.EnsureUserBucketExistsAsync(userStorageId);
                var uri = await _storage.UploadAsync(file, container: "", userStorageId, folder);
                if(uri == null)
                {
                    return BadRequest("User doesnt have a storage");
                }
                return Ok(new { Url = uri.ToString() });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("move")]
        public async Task<IActionResult> MoveFile([FromQuery] string userStorageId, [FromQuery] string oldPath, [FromQuery] string newPath)
        {
            var us = await _userStorage.Find(int.Parse(userStorageId));
            if (us == null)
            {
                return BadRequest("Storage not Buyed or isnt active yet");
            }

            var token = Request.Headers["Authorization"].ToString();


            token = token.Substring("Bearer ".Length).Trim();

            UserStorageVM vm = new UserStorageVM();
            // create http content to send
            // send request using post
            var json = JsonSerializer.Serialize(new { token = token, UserId = us.UserID });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized();
            }


            if (string.IsNullOrWhiteSpace(oldPath) || string.IsNullOrWhiteSpace(newPath))
                return BadRequest("Both oldPath and newPath are required.");

            await _storage.MoveFileAsync(userStorageId, oldPath, newPath);
            return Ok(new { Message = $"File moved from '{oldPath}' to '{newPath}'" });
        }

        [HttpPost("move-folder")]
        public async Task<IActionResult> MoveFolder([FromQuery] string userStorageId, [FromQuery] string oldFolder, [FromQuery] string newFolder)
        {
            var us = await _userStorage.Find(int.Parse(userStorageId));
            if (us == null)
            {
                return BadRequest("Storage not Buyed or isnt active yet");
            }

            var token = Request.Headers["Authorization"].ToString();


            token = token.Substring("Bearer ".Length).Trim();

            UserStorageVM vm = new UserStorageVM();
            // create http content to send
            // send request using post
            var json = JsonSerializer.Serialize(new { token = token, UserId = us.UserID });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized();
            }


            if (string.IsNullOrWhiteSpace(oldFolder) || string.IsNullOrWhiteSpace(newFolder))
                return BadRequest("Both oldFolder and newFolder are required.");

            await _storage.MoveFolderAsync(userStorageId, oldFolder, newFolder);
            return Ok(new { Message = $"Folder moved from '{oldFolder}' to '{newFolder}'" });
        }

        [HttpDelete("delete-folder")]
        public async Task<IActionResult> DeleteFolder([FromQuery] string userStorageId, [FromQuery] string folderPath)
        {
            var us = await _userStorage.Find(int.Parse(userStorageId));
            if (us == null)
            {
                return BadRequest("Storage not Buyed or isnt active yet");
            }

            var token = Request.Headers["Authorization"].ToString();


            token = token.Substring("Bearer ".Length).Trim();

            UserStorageVM vm = new UserStorageVM();
            // create http content to send
            // send request using post
            var json = JsonSerializer.Serialize(new { token = token, UserId = us.UserID });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized();
            }


            if (string.IsNullOrWhiteSpace(folderPath))
                return BadRequest("Folder path is required.");

            await _storage.DeleteFolderAsync(userStorageId, folderPath);
            return Ok(new { Message = $"Folder '{folderPath}' deleted for user {userStorageId}" });
        }


        [HttpDelete("Delete-Storage")]
        public async Task<IActionResult> DeleteBucket(string userStorageId)
        {
            var us = await _userStorage.Find(int.Parse(userStorageId));
            if (us == null)
            {
                return BadRequest("Storage not Buyed or isnt active yet");
            }

            var token = Request.Headers["Authorization"].ToString();


            token = token.Substring("Bearer ".Length).Trim();

            UserStorageVM vm = new UserStorageVM();
            // create http content to send
            // send request using post
            var json = JsonSerializer.Serialize(new { token = token, UserId = us.UserID });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized();
            }


            if (string.IsNullOrWhiteSpace(userStorageId))
            {
                return BadRequest("userStorageId is needed");
            }

            await _storage.DeleteBucket(userStorageId);

            return Ok("Delete Successfuly");
        }


        [HttpGet("Create-Public-Download")]
        public async Task<IActionResult> MakeDownloadPublic(string userStorageId,string filelink)
        {
            var us = await _userStorage.Find(int.Parse(userStorageId));
            if (us == null)
            {
                return BadRequest("Storage not Buyed or isnt active yet");
            }

            if (!us.IsPublic)
            {
                var token = Request.Headers["Authorization"].ToString();


                token = token.Substring("Bearer ".Length).Trim();

                UserStorageVM vm = new UserStorageVM();
                // create http content to send
                // send request using post
                var json = JsonSerializer.Serialize(new { token = token, UserId = us.UserID });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return Unauthorized();
                }
            }

            if (string.IsNullOrWhiteSpace(userStorageId))
            {
                return BadRequest("userStorageId is needed");
            }

            return Ok( $"http://localhost:5000/storage/public/download{await _storage.MakePublicDownloadLink(userStorageId, filelink)}");
        }
    }

}
