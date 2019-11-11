using System;

namespace Gates.ClientCore.ExternalCommands
{
    public interface IExternalCommandsProvider : IDisposable
    {
        bool IsOpened { get; }

	    event Action<string> NewCommand;

	    void Open();

	    void Close();
    }
}
