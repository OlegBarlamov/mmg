using System;

namespace Gates.ClientCore.ExternalCommands
{
    internal class ClientHostException : Exception
    {
        internal ClientHostException(string message)
            : base(message)
        {
        }
    }
}
