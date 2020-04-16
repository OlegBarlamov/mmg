namespace Console.Core.Implementations.ExternalProcess.ProcessMessages
{
    public interface IConsoleProcessMessageFormatter
    {
        byte[] Serialize(IConsoleProcessMessage message);

        IConsoleProcessMessage Deserialize(byte[] data);
    }
}