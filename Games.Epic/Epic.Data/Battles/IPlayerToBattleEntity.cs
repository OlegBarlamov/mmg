using System;

namespace Epic.Data.Battles
{
    public interface IPlayerToBattleEntity
    {
        Guid Id { get; }
        Guid PlayerId { get; }
        Guid BattleId { get; }
        bool RansomClaimed { get; }
    }

    public class PlayerToBattleEntity : IPlayerToBattleEntity
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid BattleId { get; set; }
        public bool RansomClaimed { get; set; }
    }
}