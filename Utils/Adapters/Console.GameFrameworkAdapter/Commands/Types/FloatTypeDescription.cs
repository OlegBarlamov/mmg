namespace Console.FrameworkAdapter.Commands.Types
{
    public class FloatTypeDescription : IConsoleCommandTypeDescription
    {
        public string Title { get; } = "Float";
        public object Parse(string parameter)
        {
            return float.Parse(parameter);
        }

        public bool IsParsable(string parameter)
        {
            return float.TryParse(parameter, out _);
        }
    }
}