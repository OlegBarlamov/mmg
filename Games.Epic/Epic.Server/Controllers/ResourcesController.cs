using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.GameResources;
using Epic.Server.Authentication;
using Epic.Server.Resources;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/resources")]
    public class ResourcesController : ControllerBase
    {
        public IGameResourcesRepository ResourcesRepository { get; }

        public ResourcesController([NotNull] IGameResourcesRepository resourcesRepository)
        {
            ResourcesRepository = resourcesRepository ?? throw new ArgumentNullException(nameof(resourcesRepository));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetResources()
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);

            var resources = await ResourcesRepository.GetAllResourcesByPlayer(playerId);
            return Ok(resources.Select(x => new ResourceDashboardResource(x)));
        }
    }
}
