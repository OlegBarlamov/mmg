namespace Epic.Core.Logic
{
    public class BattleResult
    {
        public bool Finished { get; set; }
        public InBattlePlayerNumber? Winner { get; set; }
    }
}