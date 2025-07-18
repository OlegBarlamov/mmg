using System;

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
        public string CommandId { get; }
        public abstract string Command { get; }
        public int TurnNumber { get; }

        public BaseCommandFromServer(int turnNumber)
        {
            TurnNumber = turnNumber;
            CommandId = Guid.NewGuid().ToString();
        }
    }
    
    public abstract class PlayerCommandFromServer : BaseCommandFromServer
    {
        public string Player { get; }
        
        protected PlayerCommandFromServer(int turnNumber, InBattlePlayerNumber player) : base(turnNumber)
        {
            Player = player.ToString();
        }
    }
    
    public abstract class UnitCommandFromServer : PlayerCommandFromServer
    {
        public string ActorId { get; set; }
        
        protected UnitCommandFromServer(int turnNumber, InBattlePlayerNumber player, string actorId) : base(turnNumber, player)
        {
            ActorId = actorId;
        }
    }
}