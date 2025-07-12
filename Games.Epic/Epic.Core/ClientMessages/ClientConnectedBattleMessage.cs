namespace Epic.Core.ClientMessages
{
    public class ClientConnectedBattleMessage : IClientBattleMessage
    {
        public string CommandId { get; set; }
        public string Command { get; set; } = ClientBattleCommands.CLIENT_CONNECTED;
        public PlayerNumber Player { get; set; }
        public int TurnIndex { get; set; }
    }
}