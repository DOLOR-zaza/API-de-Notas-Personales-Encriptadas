using Microsoft.AspNetCore.Mvc;

namespace API_BACKEND1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "OK" });
        }
    }
}
