using System;
using System.Threading.Tasks;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.Magic;
using Epic.Core.Services.Players;
using Epic.Server.Authentication;
using Epic.Server.Resources;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/heroes")]
    public class HeroesController : ControllerBase
    {
        public IPlayersService PlayersService { get; }
        public IHeroesService HeroesService { get; }
        public IMagicsService MagicsService { get; }

        public HeroesController(
            [NotNull] IPlayersService playersService,
            [NotNull] IHeroesService heroesService,
            [NotNull] IMagicsService magicsService)
        {
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            HeroesService = heroesService ?? throw new ArgumentNullException(nameof(heroesService));
            MagicsService = magicsService ?? throw new ArgumentNullException(nameof(magicsService));
        }
        
        [HttpGet("known-magic")]
        public async Task<IActionResult> GetKnownMagic()
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);

            var player = await PlayersService.GetById(playerId);
            if (player.ActiveHero == null)
                return BadRequest("Active hero is not set.");

            var hero = await HeroesService.GetById(player.ActiveHero.Id);
            var heroStats = hero.GetCumulativeHeroStats();
            var variables = MagicExpressionsVariables.FromHero(heroStats);

            var magicTypeIds = hero.KnownMagicTypeIds ?? Array.Empty<Guid>();
            var result = new MagicResource[magicTypeIds.Length];
            for (var i = 0; i < magicTypeIds.Length; i++)
            {
                var magic = await MagicsService.Create(magicTypeIds[i], variables);
                result[i] = new MagicResource(magic);
            }

            return Ok(result);
        }
    }
}
