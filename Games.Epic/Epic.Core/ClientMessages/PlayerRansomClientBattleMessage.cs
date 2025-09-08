namespace Epic.Core.ClientMessages
{
    public class PlayerRansomClientBattleMessage : IClientBattleMessage
    {
        public string CommandId { get; set; }
        public string Command { get; set; } = ClientBattleCommands.PLAYER_RANSOM;
        public InBattlePlayerNumber Player { get; set; }
        public int TurnIndex { get; set; }
    }
}
