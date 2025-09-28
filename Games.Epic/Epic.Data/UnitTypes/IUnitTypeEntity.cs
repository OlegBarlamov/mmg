using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Data.UnitTypes.Subtypes;

namespace Epic.Data.UnitTypes
{
    public interface IUnitProps
    {
        int Speed { get; }
        int Health { get; }
        int Attack { get; }
        int Defense { get; }
        MovementType Movement { get; }
        IReadOnlyList<IAttackFunctionType> Attacks { get; }
    }
    
    public interface IUnitTypeProperties : IUnitProps
    {
        string Key { get; }
        string Name { get; }
        string BattleImgUrl { get; }
        string DashboardImgUrl { get; }
        string DwellingImgUrl { get; }
        int Value { get; }
        int ToTrainAmount { get; }
        IReadOnlyDictionary<string, int> ResourcesDistribution { get; }
        IReadOnlyList<Guid> UpgradeForUnitTypeIds { get; }
    }

    public abstract class UnitTypeProps : IUnitProps
    {
        public int Speed { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public MovementType Movement { get; set; }
        public List<AttackFunctionType> Attacks { get; set; } = new List<AttackFunctionType>();

        IReadOnlyList<IAttackFunctionType> IUnitProps.Attacks => Attacks;
        
        protected UnitTypeProps() { }
    } 
    
    public class UnitTypeProperties : UnitTypeProps, IUnitTypeProperties {
        public string Key { get; set; }
        public string Name { get; set; }
        public string BattleImgUrl { get; set; }
        public string DashboardImgUrl { get; set; }
        public string DwellingImgUrl { get; set; }
        public int Value { get; set; } = 1;
        public int ToTrainAmount { get; set; }
        public Dictionary<string, int> ResourcesDistribution { get; set; } = new Dictionary<string, int>();
        public List<Guid> UpgradeForUnitTypeIds { get; set; } = new List<Guid>();
        
        IReadOnlyList<Guid> IUnitTypeProperties.UpgradeForUnitTypeIds => UpgradeForUnitTypeIds;

        IReadOnlyDictionary<string, int> IUnitTypeProperties.ResourcesDistribution => ResourcesDistribution; 
        
        public UnitTypeProperties() { }
    }
    
    public interface IUnitTypeEntity : IUnitTypeProperties
    {
        Guid Id { get; }
    }

    public class UnitTypeEntity : UnitTypeProperties, IUnitTypeEntity
    {
        public Guid Id { get; }

        private UnitTypeEntity(Guid id)
        {
            Id = id;
        }

        internal void FillProperties(IUnitTypeProperties properties)
        {
            Key = properties.Key;
            Name = properties.Name;
            BattleImgUrl = properties.BattleImgUrl;
            DashboardImgUrl = properties.DashboardImgUrl;
            Speed = properties.Speed;
            Health = properties.Health;
            Attacks = properties.Attacks.Cast<AttackFunctionType>().ToList();
            Value = properties.Value;
            ResourcesDistribution = new Dictionary<string, int>(properties.ResourcesDistribution);
            ToTrainAmount = properties.ToTrainAmount;
            Attack = properties.Attack;
            Defense = properties.Defense;
            Movement = properties.Movement;
            UpgradeForUnitTypeIds = new List<Guid>(properties.UpgradeForUnitTypeIds);
        }

        public static UnitTypeEntity FromProperties(Guid id, IUnitTypeProperties properties)
        {
            var entity = new UnitTypeEntity(id);
            entity.FillProperties(properties);
            return entity;
        }
    }
}