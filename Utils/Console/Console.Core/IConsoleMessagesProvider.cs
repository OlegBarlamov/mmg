using System;
using Console.Core.Models;

namespace Console.Core
{
    public interface IConsoleMessagesProvider
    {
        event Action NewMessages;
        
        bool IsQueueEmpty { get; }

        IConsoleMessage Pop();
    }
}