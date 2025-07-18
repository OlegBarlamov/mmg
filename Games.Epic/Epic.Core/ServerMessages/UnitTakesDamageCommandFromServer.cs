namespace Epic.Core.ServerMessages
{
    public class UnitTakesDamageCommandFromServer : UnitCommandFromServer
    {
        public UnitTakesDamageCommandFromServer(int turnNumber, InBattlePlayerNumber player, string actorId) : base(turnNumber, player, actorId)
        {
        }

        public override string Command => "TAKE_DAMAGE";
        public int DamageTaken { get; set; }
        public int KilledCount { get; set; }
        public int RemainingCount { get; set; }
        public int RemainingHealth { get; set; }
    }
}