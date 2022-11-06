using System;

namespace BoardPlatform.Server.Services
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