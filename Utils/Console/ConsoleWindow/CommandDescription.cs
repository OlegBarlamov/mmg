using System;

namespace ConsoleWindow
{
    public class CommandDescription
    {
        public string CommandName { get; }

        public string CommandSignature { get; set; }

        public string Description { get; set; }

        public CommandDescription(string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName)) throw new ArgumentException(nameof(commandName));

            CommandName = commandName;
            CommandSignature = commandName;
            Description = commandName;
        }
    }
}
