using System;

namespace Epic.Data.Heroes
{
    public interface IHeroStats
    {
        int Attack { get; }
        int Defense { get; }
        int Level { get; }
        int Experience { get; }
        int Power { get; }
        int Knowledge { get; }
        int CurrentMana { get; }
    }
    
    public interface IHeroEntityFields : IHeroStats
    {
        string Name { get; }
        Guid ArmyContainerId { get; }
        bool IsKilled { get; }
    }
    
    public interface IHeroEntity : IHeroEntityFields
    {
        Guid Id { get; }
    }

    public class MutableHeroEntityFields : IHeroEntityFields
    {
        public string Name { get; set; }
        public Guid ArmyContainerId { get; set; }
        public bool IsKilled { get; set; }
        public int Level { get; set; } = 1;
        public int Experience { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Power { get; set; }
        public int Knowledge { get; set; }
        public int CurrentMana { get; set; }

        public MutableHeroEntityFields() { }
    }

    public class MutableHeroEntity : MutableHeroEntityFields, IHeroEntity
    {
        public Guid Id { get; set; }

        private MutableHeroEntity(Guid id)
        {
            Id = id;
        }

        internal MutableHeroEntity CopyFrom(IHeroEntityFields fields)
        {
            Name = fields.Name;
            ArmyContainerId = fields.ArmyContainerId;
            IsKilled = fields.IsKilled;
            Level = fields.Level;
            Experience = fields.Experience;
            Attack = fields.Attack;
            Defense = fields.Defense;
            Power = fields.Power;
            Knowledge = fields.Knowledge;
            CurrentMana = fields.CurrentMana;
            
            return this;
        }

        public static MutableHeroEntity FromFields(Guid id, IHeroEntityFields fields)
        {
            return new MutableHeroEntity(id).CopyFrom(fields);
        } 
    } 
}