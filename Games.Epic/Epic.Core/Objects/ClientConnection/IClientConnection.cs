using System;
using System.Threading.Tasks;

namespace Epic.Core.Objects.ClientConnection
{
    public interface IClientConnection : IDisposable
    {
        event Action<string> OnMessageReceived;
    
        Guid ConnectionId { get; }
        bool IsActive { get; }
    
        Task SendMessageAsync(string message);
    
        Task ListenMessagesAsync();
    
        Task CloseAsync();
    }
}