namespace Epic.Core.ServerMessages
{
    public class BattleFinishedCommandFromServer : BaseCommandFromServer 
    {
        public BattleFinishedCommandFromServer(int turnNumber) : base(turnNumber)
        {
        }

        public string Winner { get; set; }
        public override string Command { get; } = "BATTLE_FINISHED";
    }
}