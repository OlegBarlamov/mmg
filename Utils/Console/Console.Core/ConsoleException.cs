using System;

namespace Console.Core
{
    public class ConsoleException : Exception
    {
        internal ConsoleException(string message)
            : base(message)
        {
        }

        internal ConsoleException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}