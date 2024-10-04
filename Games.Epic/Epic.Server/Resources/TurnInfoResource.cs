namespace Epic.Server.Resources
{
    public class TurnInfoResource
    {
        public int Index { get; }
        public int PlayerIndex { get; }
        
        public TurnInfoResource(int index, int playerIndex)
        {
            Index = index;
            PlayerIndex = playerIndex;
        }
    }
}