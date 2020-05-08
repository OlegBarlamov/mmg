using System;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing
{
    public interface IGraphicsPipelineProcessor : IDisposable
    {
        void SetupComponents([NotNull, ItemNotNull] IReadOnlyObservableList<IGraphicComponent> componentsCollection);
        
        void Process(GameTime gameTime, IPipelineContext context, [NotNull] IGraphicsPipelineAction pipelineAction);
    }
}