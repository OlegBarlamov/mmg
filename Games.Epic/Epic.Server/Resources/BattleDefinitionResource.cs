using System;
using System.Linq;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Players;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Units;
using Epic.Logic;

namespace Epic.Server.Resources
{
    public class BattleDefinitionResource
    {
        public string Id { get; }
        public int Width { get; }
        public int Height { get; }
        
        public BattleDefinitionRewardResource[] Rewards { get; }
        public BattleDefinitionUnitResource[] Units { get; }
        
        public int? ExpiresAfterDays { get; }
        
        public BattleDefinitionResource(
            IBattleDefinitionObject battleDefinitionObject, 
            IRewardObject[] rewards, 
            DescriptionVisibility rewardVisibility,
            DescriptionVisibility guardVisibility,
            Guid goldResourceId,
            IPlayerObject playerObject = null)
        {
            Id = battleDefinitionObject.Id.ToString();
            Width = battleDefinitionObject.Width;
            Height = battleDefinitionObject.Height;
            Rewards = rewards.Select(x => new BattleDefinitionRewardResource(x, rewardVisibility, goldResourceId)).ToArray();
            ExpiresAfterDays = playerObject != null ? battleDefinitionObject.ExpireAtDay - playerObject.Day : null;
            
            if (battleDefinitionObject.Units.Count > 0)
            {
                Units = CombinedUnitDescriptor.Create(battleDefinitionObject.Units)
                    .Select(x => new BattleDefinitionUnitResource(x, guardVisibility))
                    .ToArray();
            }
            else
            {
                Units = Array.Empty<BattleDefinitionUnitResource>();
            }
        }
    }
    
    public class BattleDefinitionRewardResource
    {
        public string Name { get; }
        public string ThumbnailUrl { get; }
        public string Amount { get; }

        public BattleDefinitionRewardResource(IRewardObject rewardObject, DescriptionVisibility visibility, Guid goldResourceId)
        {
            var description = RewardDescription.Create(rewardObject as CompositeRewardObject, visibility, goldResourceId);
            
            Name = description.Name;
            ThumbnailUrl = description.IconUrl;
            Amount = description.Amount;
        }
    }

    public class BattleDefinitionUnitResource
    {
        public string Name { get; }
        public string ThumbnailUrl { get; }
        public string Count { get; }

        public BattleDefinitionUnitResource(IGlobalUnitObject globalUnitObject, DescriptionVisibility visibility)
        {
            var description = ArmySizeDescription.Create(globalUnitObject, visibility);
            Name = description.Name;
            ThumbnailUrl = description.ThumbnailUrl;
            Count = description.ArmySize;
        }
    }
}