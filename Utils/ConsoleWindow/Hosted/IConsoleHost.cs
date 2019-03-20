using System;

// ReSharper disable once CheckNamespace
namespace ConsoleWindow
{
	public interface IConsoleHost : IConsole, IDisposable
	{
		event EventHandler<int> KeyPressed;

		void Invoke(Action action);

		T Invoke<T>(Func<T> func);
	}
}
