namespace Console.Core.Commands.Types
{
    public class IntTypeDescription : IConsoleCommandTypeDescription
    {
        public string Title { get; } = "Integer";
        
        public object Parse(string parameter)
        {
            return int.Parse(parameter);
        }

        public bool IsParsable(string parameter)
        {
            return int.TryParse(parameter, out _);
        }
    }
}