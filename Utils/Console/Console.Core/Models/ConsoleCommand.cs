namespace Console.Core.Models
{
    public class ConsoleCommand : IConsoleCommand
    {
        public string Text { get; }
        
        public IConsoleCommandMetadata Metadata { get; }
        
        public ConsoleCommand(string command, string title, string description)
        {
            Text = command;
            Metadata = new ConsoleCommandMetadata(title, description);
        }

        private class ConsoleCommandMetadata : IConsoleCommandMetadata
        {
            public string Title { get; }
            public string Description { get; }
            public object Data { get; }

            public ConsoleCommandMetadata(string title, string description)
            {
                Title = title;
                Description = description;
            }
        }
    }
}