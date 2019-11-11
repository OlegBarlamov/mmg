using System;

namespace Gates.ClientCore.ExternalCommands
{
    internal class ExternalCommand
    {
        public bool IsEmpty => string.IsNullOrWhiteSpace(Name);

        public string Name { get; }

        public string[] Parameters { get; }

        public ExternalCommand(string name, params string[] parameters)
        {
            Name = name;
            Parameters = parameters ?? Array.Empty<string>();
        }

        public static ExternalCommand Empty()
        {
            return new ExternalCommand(string.Empty);
        }
    }
}
