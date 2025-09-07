using System;
using System.Linq;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Players;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Units;
using Epic.Logic.Descriptions;

namespace Epic.Server.Resources
{
    public class BattleDefinitionResource
    {
        public string Id { get; }
        public int Width { get; }
        public int Height { get; }
        
        public BattleDefinitionRewardResource[] Rewards { get; }
        public BattleDefinitionUnitResource[] Units { get; }
        public bool IsFinished { get; }
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
            IsFinished = battleDefinitionObject.IsFinished;
            Width = battleDefinitionObject.Width;
            Height = battleDefinitionObject.Height;
            Rewards = rewards.SelectMany(x => BattleDefinitionRewardResource.CreateFromComposite((CompositeRewardObject)x, rewardVisibility, goldResourceId))
                .ToArray();
            ExpiresAfterDays = playerObject != null ? battleDefinitionObject.ExpireAtDay - playerObject.Day : null;
            
            if (battleDefinitionObject.Units.Count > 0)
            {
                Units = CombinedUnitDescriptor.Create(battleDefinitionObject.Units, true)
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
        public string Description { get; }

        public BattleDefinitionRewardResource(RewardDescription description)
        {
            Name = description.Name;
            ThumbnailUrl = description.IconUrl;
            Amount = description.Amount;
            Description = description.Tooltip;
        }

        public static BattleDefinitionRewardResource[] CreateFromComposite(CompositeRewardObject rewardObject, DescriptionVisibility visibility, Guid goldResourceId)
        {
            var descriptions = RewardDescription.CreateComposite(rewardObject, visibility, goldResourceId);
            return descriptions.Select(x => new BattleDefinitionRewardResource(x)).ToArray();
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