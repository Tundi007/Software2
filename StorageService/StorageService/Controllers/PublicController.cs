using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StorageService.Application.Interfaces;

namespace StorageService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly ICloudStorageService _storage;

        public PublicController(ICloudStorageService storage)
        {
            _storage = storage;
        }

        [HttpGet("download/{bucket}/{file}")]
        public async Task<IActionResult> Download(string bucket, string file)
        {
            var stream = await _storage.DownloadAsync(file, "", bucket);
            return File(stream, "application/octet-stream", file);
        }
    }
}
