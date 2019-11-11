namespace Gates.ClientCore.ExternalCommands
{
    public interface IExternalCommandsProcessor
    {
        void ProcessCommand(string command);
    }
}