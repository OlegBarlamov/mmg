using Epic.Core;

namespace Epic.Server.Resources
{
    public class TurnInfoResource
    {
        public int Index { get; }
        public string Player { get; }
        
        public TurnInfoResource(int index, int playerIndex)
        {
            Index = index;
            Player = ((PlayerNumber)playerIndex).ToString();
        }
    }
}