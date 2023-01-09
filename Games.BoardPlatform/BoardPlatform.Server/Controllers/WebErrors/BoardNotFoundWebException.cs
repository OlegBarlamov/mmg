using System.Net;
using System.Web.Http;

namespace BoardPlatform.Server.Controllers.WebErrors
{
    
    public class BoardNotFoundWebException : HttpResponseException
    {
        public BoardNotFoundWebException() : base(HttpStatusCode.NotFound)
        {
            
        }
    }
}