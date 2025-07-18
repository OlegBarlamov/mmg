using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.Services.Rewards;
using Epic.Server.Authentication;
using Epic.Server.RequestBodies;
using Epic.Server.Resources;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/rewards")]
    public class RewardsController : ControllerBase
    {
        private IRewardsService RewardsService { get; }

        public RewardsController([NotNull] IRewardsService rewardsService)
        {
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCurrentNotAcceptedRewards()
        {
            var userId = User.GetId();
            var rewards = await RewardsService.GetNotAcceptedUserRewards(userId);
            var rewardResources = rewards.Select(x => new AcceptingRewardResource(x));
            return Ok(rewardResources);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> AcceptReward(Guid id, [FromBody] AcceptRewardRequestBody body)
        {
            if (body.Accepted)
            {
                var acceptResult = await RewardsService.AcceptRewardAsync(id, User.GetId(), body.Amounts);
                return Ok(new AcceptedRewardResource(acceptResult));
            }

            var rejectResult = await RewardsService.RejectRewardAsync(id, User.GetId());
            return Ok(new AcceptedRewardResource(rejectResult));
        }
    }
}