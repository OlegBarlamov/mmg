namespace Epic.Core.ServerMessages
{
    public interface IServerBattleMessage
    {
        string CommandId { get; }
        string Command { get; }
        int TurnNumber { get; }
    }
    
    public abstract class BaseCommandFromServer : IServerBattleMessage
    {
        public string CommandId { get; set; }
        public abstract string Command { get; }
        public int TurnNumber { get; set; }
    }
    
    public abstract class PlayerCommandFromServer : BaseCommandFromServer
    {
        public string Player { get; set; }
    }
    
    public abstract class UnitCommandFromServer : PlayerCommandFromServer
    {
        public string ActorId { get; set; }
    }
}