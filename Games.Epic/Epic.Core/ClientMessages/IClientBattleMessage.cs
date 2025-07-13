namespace Epic.Core.ClientMessages
{
    public interface IClientBattleMessage
    {
        string CommandId { get; }
        string Command { get; }
        InBattlePlayerNumber Player { get; }
        int TurnIndex { get; }
    }

    public static class ClientBattleCommands
    {
        public const string CLIENT_CONNECTED = "CLIENT_CONNECTED";
        public const string UNIT_ATTACK = "UNIT_ATTACK";
        public const string UNIT_MOVE = "UNIT_MOVE";
    }
}