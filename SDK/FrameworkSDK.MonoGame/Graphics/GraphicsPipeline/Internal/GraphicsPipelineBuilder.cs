using System;
using System.Collections.Generic;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.MonoGame.Graphics.Services;
using FrameworkSDK.MonoGame.Localization;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using NetExtensions.Collections;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    internal class GraphicsPipelineBuilder : IGraphicsPipelineBuilder
    {
        public IRenderTargetsFactoryService RenderTargetsFactoryService { get; } =  AppContext.ServiceLocator.Resolve<IRenderTargetsFactoryService>();
        public IDisplayService DisplayService { get; } = AppContext.ServiceLocator.Resolve<IDisplayService>();
        
        public GraphicsDevice GraphicsDevice { get; }
        public IFrameworkLogger FrameworkLogger { get; }

        private IGraphicsPipelinePassAssociateService GraphicsPipelinePassAssociateService { get; }
        private readonly IReadOnlyObservableList<IGraphicComponent> _graphicComponents;
        private readonly List<IGraphicsPipelineAction> _actions = new List<IGraphicsPipelineAction>();
        private bool _built;

        public GraphicsPipelineBuilder(
        [NotNull] IReadOnlyObservableList<IGraphicComponent> graphicComponents, 
        [NotNull] IGraphicsPipelinePassAssociateService graphicsPipelinePassAssociateService,
        [NotNull] GraphicsDevice graphicsDevice,
        [NotNull] IFrameworkLogger frameworkLogger)
        {
            GraphicsPipelinePassAssociateService = graphicsPipelinePassAssociateService ?? throw new ArgumentNullException(nameof(graphicsPipelinePassAssociateService));
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            FrameworkLogger = frameworkLogger ?? throw new ArgumentNullException(nameof(frameworkLogger));
            _graphicComponents = graphicComponents ?? throw new ArgumentNullException(nameof(graphicComponents));
        }

        public IGraphicsPipelineBuilder AddAction([NotNull] IGraphicsPipelineAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            
            _actions.Add(action);
            return this;
        }

        public IGraphicsPipeline Build()
        {
            if (_built) throw new FrameworkMonoGameException(Strings.Exceptions.Graphics.PipelineAlreadyBuilt);
            _built = true;
            
            var actions = new List<IGraphicsPipelineAction>(_actions);
            _actions.Clear();
            
            return new GraphicsPipeline(actions, _graphicComponents, GraphicsPipelinePassAssociateService, FrameworkLogger);
        }
    }
}