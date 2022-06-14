namespace Console.Core.Commands.Types
{
    public class StringTypeDescription : IConsoleCommandTypeDescription
    {
        public string Title { get; } = "String";
        public object Parse(string parameter)
        {
            return parameter;
        }

        public bool IsParsable(string parameter)
        {
            return true;
        }
    }
}