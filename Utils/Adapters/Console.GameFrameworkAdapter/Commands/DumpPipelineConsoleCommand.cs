using System;
using System.Threading.Tasks;
using Console.Core.Commands;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;

namespace Console.GameFrameworkAdapter.Commands
{
    [RegisterConsoleCommand]
    public class DumpPipelineConsoleCommand : FixedTypedExecutableConsoleCommand
    {
        public override string Text { get; } = "dump_pipeline";
        public override string Description { get; } = "Dump graphics pipeline pass info on next frame";

        private ICurrentSceneProvider CurrentSceneProvider { get; }

        public DumpPipelineConsoleCommand([NotNull] ICurrentSceneProvider currentSceneProvider)
        {
            CurrentSceneProvider = currentSceneProvider ?? throw new ArgumentNullException(nameof(currentSceneProvider));
        }

        protected override Task ExecuteAsync()
        {
            CurrentSceneProvider.CurrentScene.GraphicsPipeline.RequestDump();
            return Task.CompletedTask;
        }
    }
}
