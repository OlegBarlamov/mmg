using System;
using System.Threading.Tasks;
using Console.Core.Commands;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;

namespace Console.GameFrameworkAdapter.Commands
{
    [RegisterConsoleCommand]
    public class TerminateConsoleCommand : FixedTypedExecutableConsoleCommand
    {
        public override string Text { get; } = "terminate";
        public override string Description { get; } = "Terminate the program";
        
        private IAppTerminator AppTerminator { get; }

        public TerminateConsoleCommand([NotNull] IAppTerminator appTerminator)
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