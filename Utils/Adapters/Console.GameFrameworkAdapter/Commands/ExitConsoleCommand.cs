using System;
using System.Threading.Tasks;
using Console.Core.Commands;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;

namespace Console.GameFrameworkAdapter.Commands
{
    [RegisterConsoleCommand]
    public class ExitConsoleCommand : FixedTypedExecutableConsoleCommand
    {
        public override string Text { get; } = "exit";
        public override string Description { get; } = "Exit the application";
        
        private IAppTerminator AppTerminator { get; }

        public ExitConsoleCommand([NotNull] IAppTerminator appTerminator)
        {
            AppTerminator = appTerminator ?? throw new ArgumentNullException(nameof(appTerminator));
        }
        
        protected override Task ExecuteAsync()
        {
            AppTerminator.Terminate();
            return Task.CompletedTask;
        }
    }
}