using BoardPlatform.Server.Data;
using Newtonsoft.Json;

namespace BoardPlatform.Server
{
    public class IWsClientToServerMessage
    {
        public long MessageId { get; set; }
        public WsClientToServerCommand Command { get; set; }
        public object Payload { get; set; }

        /// <summary>
        /// For deserialization only
        /// </summary>
        public IWsClientToServerMessage()
        {
        }
        
        public IWsClientToServerMessage(long messageId, WsClientToServerCommand command, object payload)
        {
            MessageId = messageId;
            Command = command;
            Payload = payload;
        }
    }

    public enum WsClientToServerCommand
    {
        None,
        ConnectedHandshake,
    }
    
    public class WsClientToServerConnectedHandshakeMessage : IWsClientToServerMessage {
        private WsClientToServerConnectedHandshakeMessage(long messageId, WsClientToServerHandshakeMessagePayload payload) 
            : base(messageId, WsClientToServerCommand.ConnectedHandshake, payload)
        {
        }
    }

    public class WsClientToServerHandshakeMessagePayload
    {
        public ClientAppInfo ClientAppInfo { get; } 
    }
}