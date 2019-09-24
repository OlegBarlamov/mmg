using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.MonoGame.GameStructure;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics
{
    internal abstract class GraphicPipelineStep
    {
        public abstract void Process(GameTime gameTime, IReadOnlyCollection<IGraphicComponent> components);
    }

    internal class GraphicPipelinePassStep : GraphicPipelineStep
    {
        private IGraphicsPass GraphicsPass { get; }

        public GraphicPipelinePassStep([NotNull] IGraphicsPass graphicsPass)
        {
            GraphicsPass = graphicsPass ?? throw new ArgumentNullException(nameof(graphicsPass));
        }

        public override void Process(GameTime gameTime, IReadOnlyCollection<IGraphicComponent> components)
        {
            var targetComponents = FindTargetComponentsForPass(GraphicsPass, components);

            GraphicsPass.Process(gameTime, targetComponents);
        }

        private static IEnumerable<IGraphicComponent> FindTargetComponentsForPass(IGraphicsPass pass,
            IEnumerable<IGraphicComponent> allComponents)
        {
            return allComponents.Where(component => component.GraphicsPassName == pass.Name);
        }
    }

}