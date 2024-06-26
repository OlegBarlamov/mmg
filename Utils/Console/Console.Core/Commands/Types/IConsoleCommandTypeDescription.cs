namespace Console.Core.Commands.Types
{
    public interface IConsoleCommandTypeDescription
    {
        string Title { get; }
        object Parse(string parameter);
        bool IsParsable(string parameter);
    }
    
}