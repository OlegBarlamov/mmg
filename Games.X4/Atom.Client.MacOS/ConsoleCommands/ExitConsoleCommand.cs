using System;
using System.Threading.Tasks;
using Console.FrameworkAdapter.Commands;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;

namespace Atom.Client.MacOS.ConsoleCommands
{
    [RegisterConsoleCommand]
    public class ExitConsoleCommand : ParametrizedExecutableConsoleCommand
    {
        public override string Text { get; } = "exit";
        public override string Description { get; } = "Exit the application";
        public override string Title { get; } = "Exit";
        
        private IAppTerminator AppTerminator { get; }

        public ExitConsoleCommand([NotNull] IAppTerminator appTerminator)
        {
            AppTerminator = appTerminator ?? throw new ArgumentNullException(nameof(appTerminator));
        }
        
        protected override Task ExecuteAsyncWithParameters(string[] parameters)
        {
            AppTerminator.Terminate();
            return Task.CompletedTask;
        }
    }
}