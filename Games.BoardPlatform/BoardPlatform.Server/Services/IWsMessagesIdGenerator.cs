using System;

namespace BoardPlatform.WebClient.Services
{
    public interface IWsMessagesIdGenerator
    {
        long GenerateId();
    }

    public class WsMessagesIdGenerator : IWsMessagesIdGenerator
    {
        public long GenerateId()
        {
            return Guid.NewGuid().GetHashCode();
        }
    }
}