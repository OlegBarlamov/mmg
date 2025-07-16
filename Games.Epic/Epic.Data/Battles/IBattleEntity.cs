using System;

namespace Epic.Data.Battles
{
    public interface IBattleEntity
    {
        Guid Id { get; }
        Guid BattleDefinitionId { get; }
        int TurnIndex { get; }
        int Width { get; }
        int Height { get; }
        bool IsActive { get; }
    }

    public class MutableBattleEntity : IBattleEntity
    {
        public Guid Id { get; set; }
        public Guid BattleDefinitionId { get; set; }
        public int TurnIndex { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsActive { get; set; }

        public MutableBattleEntity()
        {
            
        }
    }
}