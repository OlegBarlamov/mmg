namespace Epic.Core.ClientMessages
{
    public class UnitMoveClientBattleMessage : IClientBattleMessage
    {
        public string CommandId { get; set; }
        public string Command { get; set; } = ClientBattleCommands.UNIT_MOVE;
        public PlayerNumber Player { get; set; }
        public int TurnIndex { get; set; }
        public string ActorId { get; set; }
        public HexoPoint MoveToCell { get; set; }
    }
}