namespace Epic.Core.ServerMessages
{
    public class UnitAttackCommandFromServer : UnitCommandFromServer
    {
        public override string Command => "UNIT_ATTACK";
        public string TargetId { get; }
        public int AttackIndex { get; }
        public bool IsCounterattack { get; }
        
        public UnitAttackCommandFromServer(
            int turnNumber, InBattlePlayerNumber player, string actorId, string targetId,
            int attackIndex, bool isCounterattack)
            : base(turnNumber, player, actorId)
        {
            TargetId = targetId;
            AttackIndex = attackIndex;
            IsCounterattack = isCounterattack;
        }
    }
}