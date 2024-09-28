using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    public class UserController : ControllerBase
    {
        [ApiController]
        [Route("api/user")]
        public class AppController : ControllerBase
        {
     
            [HttpGet]
            public string Index()
            {
                return "Test.";
            }
        }
    }
}