using System;
using System.Threading;
using System.Threading.Tasks;
using BoardPlatform.WebClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoardPlatform.WebClient.Controllers
{
    [ApiController]
    [Route("")]
    public class AppController : ControllerBase
    {
        private IWebSocketsService WebSocketsService { get; }

        public AppController(IWebSocketsService webSocketsService)
        {
            WebSocketsService = webSocketsService ?? throw new ArgumentNullException(nameof(webSocketsService));
        }
        
        [HttpGet]
        public string Index()
        {
            return "Server has started.";
        }

        [HttpGet("/ws")]
        public async Task Get()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await WebSocketsService.HandleConnectionAsync(webSocket, CancellationToken.None);
        }

        [HttpGet("/send")]
        public void Send()
        {
            WebSocketsService.Broadcast(new WsServerToClientConnectedMessage(1));
        }
    }
}