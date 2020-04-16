using System;
using System.Windows.Input;
using JetBrains.Annotations;

namespace ConsoleWindow.Common
{
	internal class CommandUI : ICommand
	{
		private Action<object> Action { get; }

		public CommandUI([NotNull] Action action)
		{
			if (action == null) throw new ArgumentNullException(nameof(action));

			Action = o => action();
		}

		public CommandUI([NotNull] Action<object> action)
		{
			Action = action ?? throw new ArgumentNullException(nameof(action));
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			Action?.Invoke(parameter);
		}

		public event EventHandler CanExecuteChanged;
	}
}