using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {

        [HttpGet]
        public string Index()
        {
            return "Test.";
        }
    }
}