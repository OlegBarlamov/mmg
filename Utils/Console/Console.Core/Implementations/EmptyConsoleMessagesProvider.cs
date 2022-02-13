using System;
using Console.Core.Models;

namespace Console.Core.Implementations
{
    public class EmptyConsoleMessagesProvider : IConsoleMessagesProvider
    {
        public event Action NewMessages;
        public bool IsQueueEmpty { get; } = true;
        public IConsoleMessage Pop()
        {
            throw new NotImplementedException();
        }
    }
}