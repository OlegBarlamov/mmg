using System;
using ConsoleWindow;

namespace Epic.Core.Services.Implementations
{
    internal class ConsoleService : IConsoleService, IDisposable
    {
        private readonly IConsoleHost _consoleHost;

        public ConsoleService()
        {
            _consoleHost = ConsoleFactory.CreateHosted();
        }

        public void Show()
        {
            _consoleHost.Show();
        }

        public void Hide()
        {
            _consoleHost.Hide();
        }

        public void Dispose()
        {
            _consoleHost.Dispose();
        }
    }
}
