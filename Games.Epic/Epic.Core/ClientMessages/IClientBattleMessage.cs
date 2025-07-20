namespace Epic.Core.ClientMessages
{
    public interface IClientBattleMessage
    {
        string CommandId { get; }
        string Command { get; }
        InBattlePlayerNumber Player { get; }
        int TurnIndex { get; }
    }
}
