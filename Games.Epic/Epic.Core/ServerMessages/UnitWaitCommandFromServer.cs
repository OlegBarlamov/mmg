namespace Epic.Core.ServerMessages
{
    public class UnitWaitCommandFromServer : UnitCommandFromServer
    {
        public override string Command => "UNIT_WAIT";
        
        public UnitWaitCommandFromServer(int turnNumber, InBattlePlayerNumber player, string actorId)
            : base(turnNumber, player, actorId)
        {
        }
    }
}
