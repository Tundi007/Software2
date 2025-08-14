using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IAM.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("/health")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
