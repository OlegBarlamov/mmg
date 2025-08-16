using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Connection;
using Epic.Core.Services.GameManagement;
using Epic.Core.Services.Players;
using Epic.Core.Services.Units;
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
        public IPlayersService PlayersService { get; }
        public IGlobalUnitsService GlobalUnitsService { get; }

        public BattleController(
            [NotNull] IBattlesService battlesService,
            [NotNull] IClientConnectionsService<WebSocket> clientConnectionService,
            [NotNull] IBattleGameManagersService battleGameManagersService,
            [NotNull] IBattleConnectionsService battleConnectionsService,
            [NotNull] ILogger<BattleController> logger,
            [NotNull] IPlayersService playersService,
            [NotNull] IGlobalUnitsService globalUnitsService)
        {
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));
            ClientConnectionService = clientConnectionService ?? throw new ArgumentNullException(nameof(clientConnectionService));
            BattleGameManagersService = battleGameManagersService ?? throw new ArgumentNullException(nameof(battleGameManagersService));
            BattleConnectionsService = battleConnectionsService ?? throw new ArgumentNullException(nameof(battleConnectionsService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            GlobalUnitsService = globalUnitsService ?? throw new ArgumentNullException(nameof(globalUnitsService));
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBattle(Guid id) 
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);

            var battleObject = await BattlesService.GetBattleById(id);
            if (!battleObject.PlayersIds.Contains(playerId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You are not authorized to see this battle."
                });
            }

            return Ok(new BattleResource(battleObject));
        }

        [HttpGet("player/{enemyPlayerId}")]
        public async Task<IActionResult> StartBattle(Guid enemyPlayerId)
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);
            if (playerId == enemyPlayerId)
                return BadRequest("Can not begin battle with the same player");
            
            var player = await PlayersService.GetById(playerId);
            if (player.ActiveHero?.HasAliveUnits != true)
                return BadRequest("This player does not have any units.");
            
            var enemyPlayer = await PlayersService.GetById(enemyPlayerId);
            if (player.ActiveHero?.HasAliveUnits != true)
                return BadRequest("The enemy player does not have any units.");

            var activeBattle = await BattlesService.FindActiveBattleByPlayerId(playerId);
            if (activeBattle != null)
                return BadRequest("This player already has an active battle.");
            
            var enemyPlayerActiveBattle = await BattlesService.FindActiveBattleByPlayerId(enemyPlayerId);
            if (enemyPlayerActiveBattle != null)
                return BadRequest("The enemy player already has an active battle.");
            
            var battleObject = await BattlesService.CreateBattleFromPlayerEnemy(player, enemyPlayer);
            battleObject = await BattlesService.BeginBattle(playerId, battleObject);
            return Ok(new BattleResource(battleObject));
        }

        [HttpPost]
        public async Task<IActionResult> StartBattle([FromBody] StartBattleRequestBody battleRequestBody)
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);
            
            var player = await PlayersService.GetById(playerId);
            if (player.ActiveHero?.HasAliveUnits != true)
                return BadRequest("This player does not have any units.");

            var activeBattle = await BattlesService.FindActiveBattleByPlayerId(playerId);
            if (activeBattle != null)
                return BadRequest("This player already has an active battle.");
                
            try
            {
                var battleObject = await BattlesService.CreateBattleFromDefinition(player, battleRequestBody.BattleDefinitionId);
                battleObject = await BattlesService.BeginBattle(playerId, battleObject);
                return Ok(new BattleResource(battleObject));
            }
            catch (EntityNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("{battleId}/ws")]
        public async Task EstablishWsConnection(Guid battleId)
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
            
            await BattleGameManagersService.GetOrCreateBattleGameManager(battleConnection);
            
            try
            {
                await clientConnection.ListenMessagesAsync();;
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