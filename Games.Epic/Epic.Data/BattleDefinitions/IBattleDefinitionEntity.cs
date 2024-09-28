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
}