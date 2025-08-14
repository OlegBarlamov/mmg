using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.Players;
using Epic.Server.Authentication;
using Epic.Server.Resources;
using Epic.Server.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayersController : ControllerBase
    {
        public IPlayersService PlayersService { get; }
        public ISessionsService SessionsService { get; }

        public PlayersController(
            [NotNull] IPlayersService playersService, 
            [NotNull] ISessionsService sessionsService)
        {
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            SessionsService = sessionsService ?? throw new ArgumentNullException(nameof(sessionsService));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllUserPlayers()
        {
            var userId = User.GetId();
            var players = await PlayersService.GetAllByUserId(userId); ;
            return Ok(players.Select(x => new PlayerResource(x, x.ActiveHero.IsKilled, x.ActiveHero.ArmyContainerId)));
        }

        [HttpGet("{playerId}")]
        public async Task<IActionResult> GetPlayer(Guid playerId)
        {
            var player = await PlayersService.GetByIdAndUserId(User.GetId(), playerId);
            return Ok(new PlayerResource(player, player.ActiveHero.IsKilled, player.ActiveHero.ArmyContainerId));
        }

        [HttpPost("{playerId}")]
        public async Task<IActionResult> SetActivePlayer(Guid playerId)
        {
            var sessionToken = User.GetSessionToken();
            var session = await SessionsService.GetSessionByToken(sessionToken);
            if (session.UserId != User.GetId())
                return Unauthorized("Invalid session");
            
            await SessionsService.SetPlayerId(session, playerId);
            
            return Ok();
        }
    }
}
