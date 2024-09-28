using System;

namespace Epic.Data.Battles
{
    public interface IBattleEntity
    {
        Guid Id { get; }
        int TurnIndex { get; }
        int Width { get; }
        int Height { get; }
        bool IsActive { get; }
    }
}