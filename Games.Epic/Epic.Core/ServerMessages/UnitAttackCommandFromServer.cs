namespace Epic.Core.ServerMessages
{
    public class UnitAttackCommandFromServer : UnitCommandFromServer
    {
        public UnitAttackCommandFromServer(int turnNumber, InBattlePlayerNumber player, string actorId, string targetId) : base(turnNumber, player, actorId)
        {
            TargetId = targetId;
        }

        public override string Command => "UNIT_ATTACK";
        public string TargetId { get; }
    }
}