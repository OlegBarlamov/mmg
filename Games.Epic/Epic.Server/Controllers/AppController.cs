using System;
using System.Threading.Tasks;
using Epic.Data.Exceptions;
using Epic.Server.Authentication;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = Epic.Server.Services.IAuthorizationService;

namespace Epic.Server.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("")]
    public class AppController : ControllerBase
    {
        private IAuthorizationService AuthorizationService { get; }

        public AppController([NotNull] IAuthorizationService authorizationService)
        {
            AuthorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        }
     
        [HttpGet]
        public string Index()
        {
            return "Server has been started.";
        }

        [HttpGet]
        [Route("login")]
        public async Task<IActionResult> Login()
        {
            var credentials = BasicAuthentication.ExtractCredentials(Request);
            if (credentials.Length >= 2)
            {
                try
                {
                    var sessionMetadata = SessionMetadataHelper.ExtractFromContext(HttpContext);
                    var sessionObject = await AuthorizationService.BasicLoginAsync(credentials[0], credentials[1], sessionMetadata);
                    Response.Cookies.Append("token", sessionObject.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(AuthenticationConstants.AuthenticationTokenExpirationHours),
                    });

                    return Ok("Authorized successfully.");
                }
                catch (EntityNotFoundException)
                {
                    Unauthorized("Credentials are invalid.");
                }
            } 
            
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{AppMetadata.Name}\"";
            return Unauthorized("Login required.");
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(AuthenticationConstants.AuthenticationTokenCookieName);
            return Ok(new { Message = "Successfully logged out." });
        }
    }
}