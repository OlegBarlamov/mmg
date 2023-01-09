namespace BoardPlatform.Server.Data
{
    public class ClientInfo
    {
        public string ConnectionId { get; }
        
        public ClientInfo(string connectionId)
        {
            ConnectionId = connectionId;
        }
    }
}