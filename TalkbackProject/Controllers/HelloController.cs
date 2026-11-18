using Microsoft.AspNetCore.Mvc;

namespace Talkback.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult isUp()
        {
            return Ok($"Hello {User.Identity.Name} accessed from {Request.Host}");
        }
    }
}
