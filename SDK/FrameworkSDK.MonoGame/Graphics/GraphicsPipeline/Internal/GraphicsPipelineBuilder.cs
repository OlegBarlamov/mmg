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
        public IRenderTargetsFactoryService RenderTargetsFactoryService { get; }

        private GraphicsDevice GraphicsDevice { get; }
        private IFrameworkLogger FrameworkLogger { get; }
        private IDebugInfoService DebugInfoService { get; }
        private IDisplayService DisplayService { get; }
        private IIndicesBuffersFactory IndicesBuffersFactory { get; }

        private IGraphicsPipelinePassAssociateService GraphicsPipelinePassAssociateService { get; }
        private readonly IReadOnlyObservableList<IGraphicComponent> _graphicComponents;
        private readonly List<IGraphicsPipelineAction> _actions = new List<IGraphicsPipelineAction>();
        private bool _built;

        public GraphicsPipelineBuilder(
        [NotNull] IReadOnlyObservableList<IGraphicComponent> graphicComponents, 
        [NotNull] IGraphicsPipelinePassAssociateService graphicsPipelinePassAssociateService,
        [NotNull] GraphicsDevice graphicsDevice,
        [NotNull] IFrameworkLogger frameworkLogger,
        [NotNull] IDebugInfoService debugInfoService,
        [NotNull] IRenderTargetsFactoryService renderTargetsFactoryService,
        [NotNull] IDisplayService displayService,
        [NotNull] IIndicesBuffersFactory indicesBuffersFactory)
        {
            GraphicsPipelinePassAssociateService = graphicsPipelinePassAssociateService ?? throw new ArgumentNullException(nameof(graphicsPipelinePassAssociateService));
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            FrameworkLogger = frameworkLogger ?? throw new ArgumentNullException(nameof(frameworkLogger));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));
            RenderTargetsFactoryService = renderTargetsFactoryService ?? throw new ArgumentNullException(nameof(renderTargetsFactoryService));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            IndicesBuffersFactory = indicesBuffersFactory ?? throw new ArgumentNullException(nameof(indicesBuffersFactory));
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
            
            return new GraphicsPipeline(actions, _graphicComponents, GraphicsPipelinePassAssociateService, FrameworkLogger, DebugInfoService);
        }

        public VertexBuffer CreateVertexBugger(VertexDeclaration vertexDeclaration, int vertexCount)
        {
            return new VertexBuffer(GraphicsDevice, vertexDeclaration, vertexCount, BufferUsage.WriteOnly);
        }

        public IndexBuffer CreateIndexBuffer(int indicesCount)
        {
            return IndicesBuffersFactory.CreateIndexBuffer(indicesCount);
        }
    }
}