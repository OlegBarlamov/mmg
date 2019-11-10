using System;

namespace Gates.ClientCore
{
    public interface IExternalCommandsProvider : IDisposable
    {
	    event Action<string> NewCommand;

	    void Open();

	    void Close();
    }
}
