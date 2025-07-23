using System;

namespace Epic.Data.Heroes
{
    public interface IHeroEntityFields
    {
        string Name { get; }
        Guid ArmyContainerId { get; }
        bool IsKilled { get; }
        int Level { get; }
        int Experience { get; }
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

        public MutableHeroEntityFields() { }
    }

    public class MutableHeroEntity : MutableHeroEntityFields, IHeroEntity
    {
        public Guid Id { get; set; }

        private MutableHeroEntity(Guid id)
        {
            Id = id;
        }

        public static MutableHeroEntity FromFields(Guid id, IHeroEntityFields fields)
        {
            return new MutableHeroEntity(id)
            {
                Name = fields.Name,
                ArmyContainerId = fields.ArmyContainerId,
                IsKilled = fields.IsKilled,
                Level = fields.Level,
                Experience = fields.Experience,
            };
        } 
    } 
}