namespace Epic.Server.Resources
{
    public class BattleRansomValueResource
    {
        public int Gold { get; }
        
        public BattleRansomValueResource(int ransomCost)
        {
            Gold = ransomCost;
        }
    }
}