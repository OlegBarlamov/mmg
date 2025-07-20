using System;
using System.Linq;
using Epic.Core;
using Epic.Core.Objects;
using Epic.Core.Objects.Rewards;
using Epic.Core.Services.BattleDefinitions;
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
        
        public BattleDefinitionResource(IBattleDefinitionObject battleDefinitionObject, IRewardObject[] rewards)
        {
            Id = battleDefinitionObject.Id.ToString();
            Width = battleDefinitionObject.Width;
            Height = battleDefinitionObject.Height;
            Units = battleDefinitionObject.Units.Select(x => new BattleDefinitionUnitResource(x)).ToArray();
            Rewards = rewards.Select(x => new BattleDefinitionRewardResource(x)).ToArray();
        }
    }
    
    public class BattleDefinitionRewardResource
    {
        public string Name { get; }
        public string IconUrl { get; }
        public string Amount { get; }

        public BattleDefinitionRewardResource(IRewardObject rewardObject)
        {
            var description = rewardObject.GetDescription();
            
            Name = description.Name;
            IconUrl = description.IconUrl;
            Amount = description.Amount;
        }
    }

    public class BattleDefinitionUnitResource
    {
        public Guid TypeId { get; }
        public string Name { get; }
        public string ImageUrl { get; }
        public int Count { get; }

        public BattleDefinitionUnitResource(IPlayerUnitObject playerUnitObject)
        {
            TypeId = playerUnitObject.UnitType.Id;
            Name = playerUnitObject.UnitType.Name;
            ImageUrl = playerUnitObject.UnitType.DashboardImgUrl;
            Count = playerUnitObject.Count;
        }
    }
}