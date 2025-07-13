namespace Epic.Core.ClientMessages
{
    public class UnitAttackClientBattleMessage : IClientBattleMessage
    {
        public string CommandId { get; set; }
        public string Command { get; set; } = ClientBattleCommands.UNIT_ATTACK;
        public InBattlePlayerNumber Player { get; set; }
        public int TurnIndex { get; set; }
        public string ActorId { get; set; }
        public string TargetId { get; set; }
        public HexoPoint MoveToCell { get; set; }
    }
}