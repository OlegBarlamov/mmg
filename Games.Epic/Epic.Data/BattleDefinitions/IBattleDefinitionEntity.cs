using System;

namespace Epic.Data.BattleDefinitions
{
    public interface IBattleDefinitionEntity
    {
        Guid Id { get; }
        int Width { get; }
        int Height { get; }
        
        Guid ContainerId { get; }
        public bool Finished { get; }
    }

    internal class BattleDefinitionEntity : IBattleDefinitionEntity
    {
        public Guid Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Guid ContainerId { get; set; }
        public bool Finished { get; set; }
    }
}