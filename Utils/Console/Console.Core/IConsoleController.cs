using System;
using Console.Core.Models;

namespace Console.Core
{
    public interface IConsoleController : IDisposable
    {
        bool IsShowed { get; }

        event Action ConsoleShowed;
        event Action ConsoleHidden;
        
        void Show();

        void Hide();
        
        void ClearCurrent();

        void ClearAll();

        void AddMessage(IConsoleMessage consoleMessage);
    }
}