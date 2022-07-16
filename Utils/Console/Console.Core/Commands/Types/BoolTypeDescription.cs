namespace Console.Core.Commands.Types
{
    public class BoolTypeDescription : IConsoleCommandTypeDescription
    {
        public string Title { get; } = "Boolean";
        public object Parse(string parameter)
        {
            return bool.Parse(parameter);
        }

        public bool IsParsable(string parameter)
        {
            return bool.TryParse(parameter, out _);
        }
    }
}