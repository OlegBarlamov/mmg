using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.GameResources;
using Epic.Core.Services.UnitTypes;
using Epic.Server.Resources;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/unit-types")]
    public class UnitTypesController : ControllerBase
    {
        public IUnitTypesService UnitTypesService { get; }
        public IGameResourcesService GameResourcesService { get; }

        public UnitTypesController([NotNull] IUnitTypesService unitTypesService, [NotNull] IGameResourcesService gameResourcesService)
        {
            UnitTypesService = unitTypesService ?? throw new ArgumentNullException(nameof(unitTypesService));
            GameResourcesService = gameResourcesService ?? throw new ArgumentNullException(nameof(gameResourcesService));
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUnitType(Guid id)
        {
            var unitTypeObject = await UnitTypesService.GetUnitTypeByIdAsync(id);
            var prices = await UnitTypesService.GetPrices(new[] { unitTypeObject });
            var resourcesAmounts = await GameResourcesService.GetResourcesAmountsFromPrices(prices);
            return Ok(new UnitTypeResource(unitTypeObject, resourcesAmounts.First()));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUnitTypes([FromQuery] Guid[] ids)
        {
            var unitTypeObjects = await UnitTypesService.GetUnitTypesByIdsAsync(ids);
            var prices = await UnitTypesService.GetPrices(unitTypeObjects);
            var resourcesAmounts = await GameResourcesService.GetResourcesAmountsFromPrices(prices);
            return Ok(unitTypeObjects.Select((x,i) => new UnitTypeResource(x, resourcesAmounts[i])));
        }
    }
}
