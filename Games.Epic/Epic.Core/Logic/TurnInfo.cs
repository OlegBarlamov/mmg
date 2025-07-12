namespace Epic.Core.Logic
{
    public class TurnInfo
    {
        public int TurnIndex { get; set; }
        public int PlayerIndex { get; set; }

        public TurnInfo(int turnIndex, int playerIndex)
        {
            TurnIndex = turnIndex;
            PlayerIndex = playerIndex;
        }
    }
}