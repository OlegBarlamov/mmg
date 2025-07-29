namespace Epic.Core.ClientMessages
{
    public class UnitPassClientBattleMessage : IClientBattleMessage
    {
        public string CommandId { get; set; }
        public string Command { get; set; } = ClientBattleCommands.UNIT_PASS;
        public InBattlePlayerNumber Player { get; set; }
        public int TurnIndex { get; set; }
        public string ActorId { get; set; }
    }
}