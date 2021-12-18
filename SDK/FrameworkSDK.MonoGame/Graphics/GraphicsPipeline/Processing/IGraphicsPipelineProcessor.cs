using System;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Collections;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing
{
    /// <summary>
    /// Instance per each pipeline in default implementation
    /// </summary>
    public interface IGraphicsPipelineProcessor : IDisposable
    {
        void SetupComponents([NotNull, ItemNotNull] IReadOnlyObservableList<IGraphicComponent> componentsCollection);
        
        void Process(GameTime gameTime, IPipelineContext context, [NotNull] IGraphicsPipelineAction pipelineAction);
    }
}