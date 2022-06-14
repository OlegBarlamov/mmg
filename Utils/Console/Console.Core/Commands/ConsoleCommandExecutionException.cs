using System;

namespace Console.Core.Commands
{
    public class ConsoleCommandExecutionException : Exception
    {
        public ConsoleCommandExecutionException(string message)
            : base(message)
        {
        }

        public ConsoleCommandExecutionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}