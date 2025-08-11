using System;
using System.Threading.Tasks;
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

        public UnitTypesController([NotNull] IUnitTypesService unitTypesService)
        {
            UnitTypesService = unitTypesService ?? throw new ArgumentNullException(nameof(unitTypesService));
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUnitType(Guid id)
        {
            var unitTypeObject = await UnitTypesService.GetUnitTypeByIdAsync(id);
            return Ok(new UnitTypeResource(unitTypeObject));
        }
    }
}
