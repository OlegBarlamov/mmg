namespace Epic.Core.ClientMessages
{
    public class PlayerRunClientBattleMessage : IClientBattleMessage
    {
        public string CommandId { get; set; }
        public string Command { get; set; } = ClientBattleCommands.PLAYER_RUN;
        public InBattlePlayerNumber Player { get; set; }
        public int TurnIndex { get; set; }
    }
}
