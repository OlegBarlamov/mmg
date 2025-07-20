using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Connection;
using Epic.Core.Services.GameManagement;
using Epic.Data.Exceptions;
using Epic.Server.Authentication;
using Epic.Server.RequestBodies;
using Epic.Server.Resources;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/battle")]
    public class BattleController : ControllerBase
    {
        public IBattlesService BattlesService { get; }
        public IClientConnectionsService<WebSocket> ClientConnectionService { get; }
        public IBattleGameManagersService BattleGameManagersService { get; }
        public IBattleConnectionsService BattleConnectionsService { get; }
        public ILogger<BattleController> Logger { get; }

        public BattleController(
            [NotNull] IBattlesService battlesService,
            [NotNull] IClientConnectionsService<WebSocket> clientConnectionService,
            [NotNull] IBattleGameManagersService battleGameManagersService,
            [NotNull] IBattleConnectionsService battleConnectionsService,
            [NotNull] ILogger<BattleController> logger)
        {
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));
            ClientConnectionService = clientConnectionService ?? throw new ArgumentNullException(nameof(clientConnectionService));
            BattleGameManagersService = battleGameManagersService ?? throw new ArgumentNullException(nameof(battleGameManagersService));
            BattleConnectionsService = battleConnectionsService ?? throw new ArgumentNullException(nameof(battleConnectionsService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCurrentUserActiveBattles()
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);
            
            var battleObject = await BattlesService.FindActiveBattleByPlayerId(playerId);
            if (battleObject == null)
                return Ok(Array.Empty<BattleResource>());

            return Ok(new [] { new BattleResource(battleObject) });
        }

        [HttpPost]
        public async Task<IActionResult> StartBattle([FromBody] StartBattleRequestBody battleRequestBody)
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);
            
            Guid battleDefinitionId;
            try
            {
                battleDefinitionId = Guid.Parse(battleRequestBody.BattleDefinitionId);
            }
            catch (FormatException)
            {
                return BadRequest($"Invalid ID format for {battleRequestBody.BattleDefinitionId}.");
            }

            try
            {
                var battleObject = await BattlesService.CreateBattleFromDefinition(playerId, battleDefinitionId);
                battleObject = await BattlesService.BeginBattle(playerId, battleObject);
                return Ok(new BattleResource(battleObject));
            }
            catch (EntityNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("{battleIdString}/ws")]
        public async Task EstablishWsConnection(string battleIdString)
        {
            if (!User.TryGetPlayerId(out var playerId))
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await HttpContext.Response.WriteAsync(Constants.PlayerIdIsNotSpecifiedErrorMessage);
                return;
            }
            
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await HttpContext.Response.WriteAsync("WebSocket connection expected.");
                return;
            }
            if (!Guid.TryParse(battleIdString, out var battleId))
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await HttpContext.Response.WriteAsync("Invalid ID format of BattleId");
                return;
            }

            var battleObject = await BattlesService.GetBattleById(battleId);
            if (!battleObject.IsActive)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await HttpContext.Response.WriteAsync("Battle is not active.");
                return;
            }

            if (!battleObject.PlayersIds.Contains(playerId))
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await HttpContext.Response.WriteAsync("No access to the battle");
                return;
            }
            
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            using var clientConnection = ClientConnectionService.CreateConnection(webSocket);
            using var battleConnection = BattleConnectionsService.CreateConnection(clientConnection, battleObject);
            
            var listenMessagesTask = clientConnection.ListenMessagesAsync(); 
            await BattleGameManagersService.GetBattleGameManager(battleConnection);

            try
            {
                await listenMessagesTask;
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
            finally
            {
                await BattleGameManagersService.RemoveClientConnection(battleConnection);
            }
        }
    }
}