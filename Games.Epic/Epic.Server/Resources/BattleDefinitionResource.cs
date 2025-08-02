using System;
using System.Linq;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Players;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Units;

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
        
        public BattleDefinitionResource(IBattleDefinitionObject battleDefinitionObject, IRewardObject[] rewards, IPlayerObject playerObject = null)
        {
            Id = battleDefinitionObject.Id.ToString();
            Width = battleDefinitionObject.Width;
            Height = battleDefinitionObject.Height;
            Units = battleDefinitionObject.Units.Select(x => new BattleDefinitionUnitResource(x)).ToArray();
            Rewards = rewards.Select(x => new BattleDefinitionRewardResource(x)).ToArray();
            ExpiresAfterDays = playerObject != null ? battleDefinitionObject.ExpireAtDay - playerObject.Day : null;
        }
    }
    
    public class BattleDefinitionRewardResource
    {
        public string Name { get; }
        public string ThumbnailUrl { get; }
        public string Amount { get; }

        public BattleDefinitionRewardResource(IRewardObject rewardObject)
        {
            var description = rewardObject.GetDescription();
            
            Name = description.Name;
            ThumbnailUrl = description.IconUrl;
            Amount = description.Amount;
        }
    }

    public class BattleDefinitionUnitResource
    {
        public Guid TypeId { get; }
        public string Name { get; }
        public string ThumbnailUrl { get; }
        public int Count { get; }

        public BattleDefinitionUnitResource(IGlobalUnitObject globalUnitObject)
        {
            TypeId = globalUnitObject.UnitType.Id;
            Name = globalUnitObject.UnitType.Name;
            ThumbnailUrl = globalUnitObject.UnitType.DashboardImgUrl;
            Count = globalUnitObject.Count;
        }
    }
}