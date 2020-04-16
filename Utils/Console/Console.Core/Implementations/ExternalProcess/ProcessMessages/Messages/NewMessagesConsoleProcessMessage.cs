using System;
using System.Collections.Generic;
using System.Linq;
using Console.Core.Models;

namespace Console.Core.Implementations.ExternalProcess.ProcessMessages.Messages
{
    [Serializable]
    internal class NewMessagesConsoleProcessMessage : IConsoleProcessMessage
    {
        public IReadOnlyCollection<IConsoleMessage> Messages { get; }
        
        public NewMessagesConsoleProcessMessage(IReadOnlyCollection<IConsoleMessage> messages)
        {
            Messages = messages;
        }
        
        public override string ToString()
        {
            if (Messages == null)
                return "NULL";

            if (Messages.Count == 1)
                return GetFullMessageInfo(Messages.First());

            var shortInfos = Messages.Select(x => $" - {GetShortMessageInfo(x)}");
            return $"Messages chunk: {Environment.NewLine} {string.Join(Environment.NewLine, shortInfos)}";
        }

        private static string GetFullMessageInfo(IConsoleMessage message)
        {
            if (message == null)
                return "NULL";

            return $"{message.Source}:{message.LogLevel}:{message.Message}:{(message.Content != null).ToString()}";
        }

        private static string GetShortMessageInfo(IConsoleMessage message)
        {
            if (message == null)
                return "NULL";

            return message.Message;
        }
    }
}