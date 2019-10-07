using System;

namespace Atom.Client.Services
{
    public interface IConsoleService
    {
        void Show();

        void Hide();

	    void WriteLine(string text, ConsoleColor color = ConsoleColor.White);

	    string ReadLine();
    }
}
