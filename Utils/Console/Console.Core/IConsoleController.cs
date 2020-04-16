using System;

namespace Console.Core
{
    public interface IConsoleController : IDisposable
    {
        bool IsShowed { get; }
        
        void Show();

        void Hide();
        
        void ClearCurrent();

        void ClearAll();
    }
}