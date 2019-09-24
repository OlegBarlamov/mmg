using System.Collections.Generic;
using FrameworkSDK.MonoGame.GameStructure;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics
{
    public class GraphicsPipeline : IGraphicsPipeline
    {
        [NotNull]
        public static GraphicsPipelineConstructor Constructor { get; } = new GraphicsPipelineConstructor();

        [NotNull]
        private IReadOnlyList<GraphicPipelineStep> Steps { get; }

        internal GraphicsPipeline(IEnumerable<GraphicPipelineStep> steps)
        {
            Steps = new List<GraphicPipelineStep>(steps);
        }

        void IGraphicsPipeline.Process(GameTime gameTime, IReadOnlyCollection<IGraphicComponent> components)
        {
            foreach (var step in Steps)
                step.Process(gameTime, components);
        }
    }
}
