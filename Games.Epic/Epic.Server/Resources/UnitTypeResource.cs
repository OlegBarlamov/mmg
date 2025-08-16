using System;
using System.Collections.Generic;
using Epic.Core.Services.UnitTypes;
using Epic.Data.UnitTypes;
using Epic.Data.UnitTypes.Subtypes;

namespace Epic.Server.Resources
{
    public class UnitTypeResource : IUnitProps
    {
        public Guid Id { get; }
        public string Name { get; }
        public string BattleImgUrl { get; }
        public string DashboardImgUrl { get; }
        
        public int Speed { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public IReadOnlyList<IAttackFunctionType> Attacks { get; set; }
        
        public UnitTypeResource(IUnitTypeObject unitTypeObject)
        {
            Id = unitTypeObject.Id;
            Name = unitTypeObject.Name;
            BattleImgUrl = unitTypeObject.BattleImgUrl;
            DashboardImgUrl = unitTypeObject.DashboardImgUrl;
            Speed = unitTypeObject.Speed;
            Health = unitTypeObject.Health;
            Attacks = unitTypeObject.Attacks;
            Attack = unitTypeObject.Attack;
            Defense = unitTypeObject.Defense;
        }
    }
}