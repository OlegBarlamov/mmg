using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Server.Authentication;
using Epic.Server.Resourses;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/battles")]
    public class BattleDefinitionsController : ControllerBase
    {
        public IBattleDefinitionsService BattleDefinitionsService { get; }

        public BattleDefinitionsController([NotNull] IBattleDefinitionsService battleDefinitionsService)
        {
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCurrentUserBattles()
        {
            var userId = User.GetId();
            var battles = await BattleDefinitionsService.GetBattleDefinitionsByUserAsync(userId);
            var battlesResources = battles.Select(x => new BattleDefinitionResource(x)).ToArray();
            return Ok(battlesResources);
        }
    }
}