using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.BattleReports;
using Epic.Server.Authentication;
using Epic.Server.Resources;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class BattleReportsController : ControllerBase
    {
        public IBattleReportsService BattleReportsService { get; }

        public BattleReportsController([NotNull] IBattleReportsService battleReportsService)
        {
            BattleReportsService = battleReportsService ?? throw new ArgumentNullException(nameof(battleReportsService));
        }
        
        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetReport(Guid reportId)
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);
            
            var reportObject = await BattleReportsService.GetById(reportId);
            if (!reportObject.PlayerIds.Contains(playerId))
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You are not authorized to see this report."
                });
            
            return Ok(new BattleReportResource(reportObject, playerId));
        }
    }
}
