using Epic.Core.ClientMessages;

namespace Epic.Core
{
    public interface IClientMessagesParserService
    {
        bool ParseSafe(string message, out IClientBattleMessage parsedMessage);
    }

    public class ClientMessagesParserService : IClientMessagesParserService
    {
        public bool ParseSafe(string message, out IClientBattleMessage parsedMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}