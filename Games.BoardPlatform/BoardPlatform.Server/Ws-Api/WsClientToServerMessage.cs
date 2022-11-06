namespace BoardPlatform.WebClient
{
    public interface IWsClientToServerMessage
    {
        public long MessageId { get; }
        public WsClientToServerCommand Command { get; }
        public object Payload { get; }
    }

    public enum WsClientToServerCommand
    {
        None,
    }
}