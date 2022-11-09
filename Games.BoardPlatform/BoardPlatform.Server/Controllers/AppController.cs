using System;
using System.Threading;
using System.Threading.Tasks;
using BoardPlatform.Server.Services;
using FrameworkSDK.Common;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BoardPlatform.Server.Controllers
{
    [ApiController]
    [Route("")]
    public class AppController : ControllerBase
    {
        private IWebSocketsService WebSocketsService { get; }
        public ILogger<AppController> Logger { get; }
        public IRandomService RandomService { get; }


        public AppController([NotNull] ILogger<AppController> logger,
            [NotNull] IRandomService randomService, IWebSocketsService webSocketsService)
        {
            WebSocketsService = webSocketsService ?? throw new ArgumentNullException(nameof(webSocketsService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
        }
        
        [HttpGet]
        public string Index()
        {
            Logger.LogInformation("This is a log message!");
            Logger.LogError("This is a error message!");
            
            Logger.LogWarning("GUID: " + RandomService.NewGuid());
            
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