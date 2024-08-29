using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("")]
    public class AppController : ControllerBase
    {
     
        [HttpGet]
        public string Index()
        {
            return "Server has been started.";
        }
    }
}