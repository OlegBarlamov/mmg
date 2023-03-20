using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.MonoGame.Graphics.Services;
using FrameworkSDK.MonoGame.Localization;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    internal class GraphicsPipelineBuilder : IGraphicsPipelineBuilder
    {
        public IRenderTargetsFactoryService RenderTargetsFactoryService { get; }
        public IVideoBuffersFactoryService VideoBuffersFactoryService { get; }

        private IFrameworkLogger FrameworkLogger { get; }
        private IDebugInfoService DebugInfoService { get; }
        
        private IReadOnlyObservableList<IGraphicComponent> GraphicComponents { get; }

        private IGraphicsPipelinePassAssociateService GraphicsPipelinePassAssociateService { get; }
        private readonly List<IGraphicsPipelineAction> _actions = new List<IGraphicsPipelineAction>();
        private bool _built;

        public GraphicsPipelineBuilder(
        [NotNull] IReadOnlyObservableList<IGraphicComponent> graphicComponents, 
        [NotNull] IGraphicsPipelinePassAssociateService graphicsPipelinePassAssociateService,
        [NotNull] IFrameworkLogger frameworkLogger,
        [NotNull] IDebugInfoService debugInfoService,
        [NotNull] IRenderTargetsFactoryService renderTargetsFactoryService,
        [NotNull] IVideoBuffersFactoryService videoBuffersFactoryService)
        {
            GraphicsPipelinePassAssociateService = graphicsPipelinePassAssociateService ?? throw new ArgumentNullException(nameof(graphicsPipelinePassAssociateService));
            FrameworkLogger = frameworkLogger ?? throw new ArgumentNullException(nameof(frameworkLogger));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));
            RenderTargetsFactoryService = renderTargetsFactoryService ?? throw new ArgumentNullException(nameof(renderTargetsFactoryService));
            VideoBuffersFactoryService = videoBuffersFactoryService ?? throw new ArgumentNullException(nameof(videoBuffersFactoryService));
            GraphicComponents = graphicComponents ?? throw new ArgumentNullException(nameof(graphicComponents));
        }

        public IGraphicsPipelineBuilder AddAction([NotNull] IGraphicsPipelineAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            
            _actions.Add(action);
            return this;
        }

        public IGraphicsPipelineBuilder InsertAction([NotNull] string actionName, [NotNull] IGraphicsPipelineAction action)
        {
            if (actionName == null) throw new ArgumentNullException(nameof(actionName));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var targetActionIndex = _actions.FindIndex(pipelineAction => pipelineAction.Name == actionName);
            _actions.Insert(targetActionIndex, action);
            return this;
        }

        public IGraphicsPipelineBuilder RemoveAction([NotNull] string actionName)
        {
            if (actionName == null) throw new ArgumentNullException(nameof(actionName));

            var targetAction = _actions.Find(action => action.Name == actionName);
            _actions.Remove(targetAction);
            return this;
        }

        public IGraphicsPipeline Build([CanBeNull] IDisposable disposable)
        {
            if (_built) throw new FrameworkMonoGameException(Strings.Exceptions.Graphics.PipelineAlreadyBuilt);
            _built = true;
            
            var actions = new List<IGraphicsPipelineAction>(_actions);
            _actions.Clear();
            
            return new GraphicsPipeline(actions, GraphicComponents, GraphicsPipelinePassAssociateService, FrameworkLogger, DebugInfoService, disposable);
        }
    }
}