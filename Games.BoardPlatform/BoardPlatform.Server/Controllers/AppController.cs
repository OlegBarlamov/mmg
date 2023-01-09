using System;
using System.Threading;
using System.Threading.Tasks;
using BoardPlatform.Data.Repositories;
using BoardPlatform.Data.Tokens;
using BoardPlatform.Server.Controllers.ResponseData;
using BoardPlatform.Server.Controllers.WebErrors;
using BoardPlatform.Server.Data;
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
        public IBoardRepository BoardRepository { get; }
        public ITokensParser TokensParser { get; }
        public IBoardSocketsWorkersManager BoardSocketsWorkersManager { get; }
        public ILogger<AppController> Logger { get; }
        public IRandomService RandomService { get; }


        public AppController([NotNull] ILogger<AppController> logger,
            [NotNull] IRandomService randomService, [NotNull] IBoardRepository boardRepository,
            [NotNull] ITokensParser tokensParser, [NotNull] IBoardSocketsWorkersManager boardSocketsWorkersManager)
        {
            BoardRepository = boardRepository ?? throw new ArgumentNullException(nameof(boardRepository));
            TokensParser = tokensParser ?? throw new ArgumentNullException(nameof(tokensParser));
            BoardSocketsWorkersManager = boardSocketsWorkersManager ?? throw new ArgumentNullException(nameof(boardSocketsWorkersManager));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
        }
        
        [HttpGet]
        public string Index()
        {
            return "Server has been started.";
        }
        
        [HttpGet("/board/{boardId}")]
        public async Task<BoardInfoResponse> GetBoardInfo(string boardId)
        {
            var boardToken = TokensParser.FromBoardId(boardId);
            try
            {
                var boardData = await BoardRepository.FindBoardAsync(boardToken);
                return new BoardInfoResponse(boardData.GetToken().GetId());
            }
            catch (Exception)
            {
                throw new BoardNotFoundWebException();
            }
        }
        
        [HttpGet("/board/{boardId}/ws")]
        public async Task EstablishWsConnection(string boardId)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            var boardToken = TokensParser.FromBoardId(boardId);
            if (!await BoardRepository.BoardExist(boardToken))
                throw new BoardNotFoundWebException();
            
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var connectionInfo = new ClientInfo(Request.HttpContext.Connection.Id);
            var boardSocketsWorker = BoardSocketsWorkersManager.GetWorker(boardToken);
            
            await boardSocketsWorker.HandleNewConnectionAsync(connectionInfo, webSocket, CancellationToken.None);
        }
    }
}