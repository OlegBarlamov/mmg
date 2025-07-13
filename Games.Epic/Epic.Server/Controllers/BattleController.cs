using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Data.Exceptions;
using Epic.Server.Authentication;
using Epic.Server.Resources;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

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

        public BattleController(
            [NotNull] IBattlesService battlesService,
            [NotNull] IClientConnectionsService<WebSocket> clientConnectionService,
            [NotNull] IBattleGameManagersService battleGameManagersService,
            [NotNull] IBattleConnectionsService battleConnectionsService)
        {
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));
            ClientConnectionService = clientConnectionService ?? throw new ArgumentNullException(nameof(clientConnectionService));
            BattleGameManagersService = battleGameManagersService ?? throw new ArgumentNullException(nameof(battleGameManagersService));
            BattleConnectionsService = battleConnectionsService ?? throw new ArgumentNullException(nameof(battleConnectionsService));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCurrentUserActiveBattles()
        {
            var userId = User.GetId();
            
            var battleObject = await BattlesService.FindActiveBattleByUserId(userId);
            if (battleObject == null)
                return Ok(Array.Empty<BattleResource>());

            return Ok(new [] { new BattleResource(battleObject) });
        }

        [HttpPost]
        public async Task<IActionResult> StartBattle(StartBattleRequestBody battleRequestBody)
        {
            var userId = User.GetId();
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
                var battleObject = await BattlesService.CreateBattleFromDefinition(userId, battleDefinitionId);
                battleObject = await BattlesService.BeginBattle(userId, battleObject);
                return Ok(new BattleResource(battleObject));
            }
            catch (EntityNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("{battleIdString}/ws")]
        public async Task<IActionResult> EstablishWsConnection(string battleIdString)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
                return BadRequest("WebSocket connection expected.");

            Guid battleId;
            try
            {
                battleId = Guid.Parse(battleIdString);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid ID format of BattleId");
            }

            var battleObject = await BattlesService.GetBattleById(battleId);
            if (!battleObject.IsActive)
                return BadRequest("Battle is not active.");
            
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            using (var clientConnection = ClientConnectionService.CreateConnection(webSocket))
            {
                using (var battleConnection = BattleConnectionsService.CreateConnection(clientConnection, battleObject))
                {
                    await BattleGameManagersService.GetBattleGameManager(battleConnection);
                    try
                    {
                        await clientConnection.ListenMessagesAsync();
                    }
                    catch (OperationCanceledException)
                    {
                        // Ignore
                    }
                    finally
                    {
                        await BattleGameManagersService.RemoveClientConnection(battleConnection);
                    }
                }
            }
            return Ok();
        }
    }
}