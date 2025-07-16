namespace Epic.Core.ServerMessages
{
    public class BattleFinishedCommandFromServer : BaseCommandFromServer 
    {
        public string Winner { get; set; }
        public override string Command { get; } = "BATTLE_FINISHED";
    }
}