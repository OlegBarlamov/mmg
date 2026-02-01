using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Services.BuffTypes;
using Epic.Core.Services.UnitTypes;
using Epic.Data.GameResources;
using Epic.Data.UnitTypes;
using Epic.Data.UnitTypes.Subtypes;

namespace Epic.Server.Resources
{
    public class UnitTypeResource
    {
        public Guid Id { get; }
        public string Name { get; }
        public string BattleImgUrl { get; }
        public string DashboardImgUrl { get; }
        
        public int Speed { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public MovementType Movement { get; set; }
        public string MovementType { get; set; }
        public IReadOnlyList<AttackTypeResource> Attacks { get; set; }
        
        public IReadOnlyList<Guid> UpgradeForUnitTypeIds { get; set; }
        
        public PriceResource Price { get; set; }
        
        public UnitTypeResource(IUnitTypeObject unitTypeObject, ResourceAmount[] resourceAmounts, IBuffTypesRegistry buffTypesRegistry)
        {
            Id = unitTypeObject.Id;
            Name = unitTypeObject.Name;
            BattleImgUrl = unitTypeObject.BattleImgUrl;
            DashboardImgUrl = unitTypeObject.DashboardImgUrl;
            Speed = unitTypeObject.Speed;
            Health = unitTypeObject.Health;
            Attacks = unitTypeObject.Attacks
                .Select(a => new AttackTypeResource(a, buffTypesRegistry))
                .ToList();
            Attack = unitTypeObject.Attack;
            Defense = unitTypeObject.Defense;
            Movement = unitTypeObject.Movement;
            MovementType = unitTypeObject.Movement.ToString();
            UpgradeForUnitTypeIds = unitTypeObject.UpgradeForUnitTypeIds;
            Price = new PriceResource(resourceAmounts);
        }
    }
}