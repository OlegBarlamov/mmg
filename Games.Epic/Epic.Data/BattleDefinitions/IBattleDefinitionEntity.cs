using System;

namespace Epic.Data.BattleDefinitions
{
    public interface IBattleDefinitionEntity
    {
        Guid Id { get; }
        int Width { get; }
        int Height { get; }
        
        Guid[] UnitsIds { get; }
    }

    internal class BattleDefinitionEntity : IBattleDefinitionEntity
    {
        public Guid Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Guid[] UnitsIds { get; set; }
    }
}