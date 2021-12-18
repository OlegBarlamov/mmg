using System;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Collections;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public interface IGraphicsPipeline : IDisposable
    {
        int ActionsCount { get; }
        IGraphicsPipelineAction this[string actionName] { get; }
        void AddAction([NotNull] IGraphicsPipelineAction action);

        void SetupComponents([NotNull, ItemNotNull] IReadOnlyObservableList<IGraphicComponent> componentsCollection);
        
        void Process(GameTime gameTime);
    }
}