namespace Epic.Server.RequestBodies
{
    public class AcceptRewardRequestBody
    {
        public int[] Amounts { get; set; }
        public bool Accepted { get; set; }
    }
}