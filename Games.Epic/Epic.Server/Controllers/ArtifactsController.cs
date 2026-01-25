using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.Artifacts;
using Epic.Core.Services.Players;
using Epic.Server.Authentication;
using Epic.Server.RequestBodies;
using Epic.Server.Resources;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/artifacts")]
    public class ArtifactsController : ControllerBase
    {
        public IPlayersService PlayersService { get; }
        public IArtifactsService ArtifactsService { get; }

        public ArtifactsController(
            [NotNull] IPlayersService playersService,
            [NotNull] IArtifactsService artifactsService)
        {
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            ArtifactsService = artifactsService ?? throw new ArgumentNullException(nameof(artifactsService));
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveHeroArtifacts()
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);

            var player = await PlayersService.GetById(playerId);
            if (player.ActiveHero == null)
                return BadRequest("Active hero is not set.");

            var artifacts = await ArtifactsService.GetByHeroId(player.ActiveHero.Id);
            return Ok(artifacts.Select(a => new ArtifactResource(a)));
        }

        [HttpPost("equip")]
        public async Task<IActionResult> Equip([FromBody] EquipArtifactRequestBody body)
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);

            var player = await PlayersService.GetById(playerId);
            if (player.ActiveHero == null)
                return BadRequest("Active hero is not set.");

            var artifact = await ArtifactsService.GetById(body.ArtifactId);
            var updated = await ArtifactsService.EquipArtifact(player.ActiveHero.Id, artifact, body.EquippedSlotsIndexes);

            // Ensure we return a fully populated artifact (including type data) regardless of equip/unequip path.
            var full = await ArtifactsService.GetById(updated.Id);
            return Ok(new ArtifactResource(full));
        }
    }
}

