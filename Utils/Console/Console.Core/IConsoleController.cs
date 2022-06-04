using System;
using Console.Core.Models;

namespace Console.Core
{
    public interface IConsoleController : IDisposable
    {
        bool IsShowed { get; }
        
        void Show();

        void Hide();
        
        void ClearCurrent();

        void ClearAll();

        void AddMessage(IConsoleMessage consoleMessage);
    }
}