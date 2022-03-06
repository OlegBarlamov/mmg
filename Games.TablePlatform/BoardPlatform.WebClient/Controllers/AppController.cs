using Microsoft.AspNetCore.Mvc;

namespace BoardPlatform.WebClient.Controllers
{
    [ApiController]
    [Route("")]
    public class AppController : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            return "Hello";
        }
    }
}