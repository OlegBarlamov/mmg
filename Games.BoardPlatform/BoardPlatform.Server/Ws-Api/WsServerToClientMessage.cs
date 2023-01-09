using System;
using System.Text.Json.Serialization;

namespace BoardPlatform.Server
{
    public interface IWsServerToClientMessage
    {
        public long MessageId { get; }
        public WsServerToClientCommand Command { get; }
        public object Payload { get; }
    }

    public enum WsServerToClientCommand
    {
        None = 0,
        ConnectedHandshake = 1,
        WidgetExist = 2,
    }
    
    [Serializable]
    public class WsServerToClientConnectedHandshakeMessage : IWsServerToClientMessage
    {
        public long MessageId { get; }
        public WsServerToClientCommand Command { get; } = WsServerToClientCommand.ConnectedHandshake;
        public object Payload { get; } = null;

        public WsServerToClientConnectedHandshakeMessage(long messageId)
        {
            MessageId = messageId;
        }
    }

    public class WsServerToClientWidgetExist : IWsServerToClientMessage
    {
        public long MessageId { get; }
        public WsServerToClientCommand Command { get; } = WsServerToClientCommand.WidgetExist;
        
        public IWidget Payload { get; }
        
        [JsonIgnore]
        object IWsServerToClientMessage.Payload => Payload;
        
        
        public WsServerToClientWidgetExist(long messageId, IWidget widget)
        {
            MessageId = messageId;
            Payload = widget;
        }
    }

    public interface IWidget : IPositioned, ISizable
    {
        public long Id { get; }
    }

    public interface IPositioned {
        public int X { get; }
        public int Y { get; }
    }
    
    public interface ISizable {
        public int Width { get; }
        public int Height { get; }
    }
}