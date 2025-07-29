using System;

namespace Epic.Data.Battles
{
    public interface IBattleEntity
    {
        Guid Id { get; }
        Guid BattleDefinitionId { get; }
        int TurnNumber { get; }
        int Width { get; }
        int Height { get; }
        bool IsActive { get; }
        int LastTurnUnitIndex { get; }
        bool ProgressDays { get; }
        int RoundNumber { get; }
    }

    public class MutableBattleEntity : IBattleEntity
    {
        public Guid Id { get; set; }
        public Guid BattleDefinitionId { get; set; }
        public int TurnNumber { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsActive { get; set; }
        
        public int LastTurnUnitIndex { get; set; }
        public bool ProgressDays { get; set; }
        public int RoundNumber { get; set; }

        public MutableBattleEntity()
        {
            
        }
    }
}