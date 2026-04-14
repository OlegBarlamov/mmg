using System;
using System.Collections.Generic;
using System.Diagnostics;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Collections;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    internal class GraphicsPipeline : IGraphicsPipeline
    {
        public static IGraphicsPipeline Empty { get; } = new EmptyGraphicsPipeline();

        public IReadOnlyList<IGraphicsPipelineAction> Actions { get; }
        private IReadOnlyObservableList<IGraphicComponent> GraphicComponents { get; }
        private IGraphicsPipelinePassAssociateService PipelinePassAssociateService { get; }
        private IDebugInfoService DebugInfoService { get; }
        [CanBeNull] private IDisposable Resource { get; }

        private readonly ModuleLogger _logger;
        
        private readonly Dictionary<string, List<IGraphicsPipelineAction>> _actionsByPassesMap = new Dictionary<string, List<IGraphicsPipelineAction>>();
        private bool _disposed;

        private bool _dumpNextFrame;

        public void RequestDump()
        {
            _dumpNextFrame = true;
        }

        private const string DebugInfoGraphicsComponentsCount = "graph_comp";

        public GraphicsPipeline(
            [NotNull] IReadOnlyList<IGraphicsPipelineAction> actions,
            [NotNull] IReadOnlyObservableList<IGraphicComponent> graphicComponents,
            [NotNull] IGraphicsPipelinePassAssociateService pipelinePassAssociateService,
            [NotNull] IFrameworkLogger frameworkLogger,
            [NotNull] IDebugInfoService debugInfoService,
            [CanBeNull] IDisposable resource)
        {
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
            GraphicComponents = graphicComponents ?? throw new ArgumentNullException(nameof(graphicComponents));
            PipelinePassAssociateService = pipelinePassAssociateService ?? throw new ArgumentNullException(nameof(pipelinePassAssociateService));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));
            Resource = resource;
            _logger = new ModuleLogger(frameworkLogger, LogCategories.Rendering);
            
            FillActionsMap();
            ProcessAlreadyExistedComponents();
            
            SubscribeToComponentsChanges(GraphicComponents);
            DebugInfoService.SetCounter(DebugInfoGraphicsComponentsCount, GraphicComponents.Count);
        }

        private void FillActionsMap()
        {
            foreach (var graphicsPipelineAction in Actions)
            {
                var passName = graphicsPipelineAction.Name;
                if (string.IsNullOrWhiteSpace(passName)) 
                    continue;
                
                if (!_actionsByPassesMap.ContainsKey(passName)) 
                    _actionsByPassesMap.Add(passName, new List<IGraphicsPipelineAction>());
                
                _actionsByPassesMap[passName].Add(graphicsPipelineAction);
            }
        }

        private void SubscribeToComponentsChanges(IReadOnlyObservableList<IGraphicComponent> components)
        {
            components.ItemAdded += ComponentsOnItemAdded;
            components.ItemRemoved += ComponentsOnItemRemoved;
        }

        private void UnsubscribeToComponentsChanges(IReadOnlyObservableList<IGraphicComponent> components)
        {
            components.ItemAdded -= ComponentsOnItemAdded;
            components.ItemRemoved -= ComponentsOnItemRemoved;
        }
        
        private void ComponentsOnItemAdded(IGraphicComponent component)
        {
            var targetPasses = PipelinePassAssociateService.GetAssociatedPasses(component);
            foreach (var targetPass in targetPasses)
            {
                AddToMap(component, targetPass);
            }
            DebugInfoService.IncrementCounter(DebugInfoGraphicsComponentsCount);
        }
        
        private void ComponentsOnItemRemoved(IGraphicComponent component)
        {
            var targetPasses = PipelinePassAssociateService.GetAssociatedPasses(component);
            foreach (var targetPass in targetPasses)
            {
                RemoveFromMap(component, targetPass);
            }
            DebugInfoService.DecrementCounter(DebugInfoGraphicsComponentsCount);
        }
        
        private void ProcessAlreadyExistedComponents()
        {
            foreach (var component in GraphicComponents)
            {
                ComponentsOnItemAdded(component);
            }
        }
        
        private void AddToMap(IGraphicComponent component, string targetPass)
        {
            if (string.IsNullOrWhiteSpace(targetPass))
                return;

            if (_actionsByPassesMap.TryGetValue(targetPass, out var graphicsPipelineActions))
            {
                foreach (var graphicsPipelineAction in graphicsPipelineActions)
                {
                    graphicsPipelineAction.OnComponentAttached(component);
                }
            }
        }

        private void RemoveFromMap(IGraphicComponent component, string targetPass)
        {
            if (string.IsNullOrWhiteSpace(targetPass))
                return;
            
            if (_actionsByPassesMap.TryGetValue(targetPass, out var graphicsPipelineActions))
            {
                foreach (var graphicsPipelineAction in graphicsPipelineActions)
                {
                    graphicsPipelineAction.OnComponentDetached(component);
                }
            }
        }
        
        public void Dispose()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(GraphicsPipeline));
            _disposed = true;
            
            UnsubscribeToComponentsChanges(GraphicComponents);

            _actionsByPassesMap.Clear();

            foreach (var pipelineAction in Actions)
            {
                try
                {
                    pipelineAction.Dispose();
                }
                catch (Exception e)
                {
                    _logger.Error($"Graphics pipeline action {pipelineAction.Name} dispose error", e);
                }
            }
            
            Resource?.Dispose();
            
            _logger.Dispose();
        }

        private IGraphicsPipelineAction _iterableAction;
        private int _iterableIndex;
        private readonly Stopwatch _dumpStopwatch = new Stopwatch();
        private readonly List<(int Index, IGraphicsPipelineAction Action, double Ms, int Rendered, int Drawn, int Meshes, int Vertices)> _dumpEntries =
            new List<(int, IGraphicsPipelineAction, double, int, int, int, int)>();

        public void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext)
        {
            var dump = _dumpNextFrame;
            _dumpNextFrame = false;

            var debugInfo = graphicDeviceContext.DebugInfoService;
            debugInfo.SetCounter(GraphicsPipelineActionBase.DebugInfoRenderingVertices, 0);
            debugInfo.SetCounter(GraphicsPipelineActionBase.DebugInfoRenderingMeshes, 0);
            debugInfo.SetCounter(GraphicsPipelineActionBase.DebugInfoRenderingComponents, 0);
            debugInfo.SetCounter(GraphicsPipelineActionBase.DebugInfoDrawComponents, 0);

            if (dump)
            {
                _dumpEntries.Clear();
            }

            for (_iterableIndex = 0; _iterableIndex < Actions.Count; _iterableIndex++)
            {
                _iterableAction = Actions[_iterableIndex];

                if (_iterableAction.IsDisabled)
                    continue;

                if (dump)
                {
                    var meshesBefore = debugInfo.GetCounter(GraphicsPipelineActionBase.DebugInfoRenderingMeshes);
                    var verticesBefore = debugInfo.GetCounter(GraphicsPipelineActionBase.DebugInfoRenderingVertices);
                    var renderCompsBefore = debugInfo.GetCounter(GraphicsPipelineActionBase.DebugInfoRenderingComponents);
                    var drawCompsBefore = debugInfo.GetCounter(GraphicsPipelineActionBase.DebugInfoDrawComponents);

                    _dumpStopwatch.Restart();
                    _iterableAction.Process(gameTime, graphicDeviceContext);
                    _dumpStopwatch.Stop();

                    _dumpEntries.Add((
                        _iterableIndex,
                        _iterableAction,
                        _dumpStopwatch.Elapsed.TotalMilliseconds,
                        debugInfo.GetCounter(GraphicsPipelineActionBase.DebugInfoRenderingComponents) - renderCompsBefore,
                        debugInfo.GetCounter(GraphicsPipelineActionBase.DebugInfoDrawComponents) - drawCompsBefore,
                        debugInfo.GetCounter(GraphicsPipelineActionBase.DebugInfoRenderingMeshes) - meshesBefore,
                        debugInfo.GetCounter(GraphicsPipelineActionBase.DebugInfoRenderingVertices) - verticesBefore));
                }
                else
                {
                    _iterableAction.Process(gameTime, graphicDeviceContext);
                }
            }

            if (dump)
            {
                var totalMs = 0.0;
                foreach (var e in _dumpEntries)
                    totalMs += e.Ms;

                _logger.Info($"=== Pipeline Dump ({Actions.Count} actions, {GraphicComponents.Count} total graphic components, {totalMs:F3}ms total) ===");
                foreach (var e in _dumpEntries)
                {
                    var pct = totalMs > 0 ? e.Ms / totalMs * 100.0 : 0;
                    _logger.Info($"  [{e.Index}] \"{e.Action.Name}\" | type={e.Action.GetType().Name} | {e.Ms:F3}ms ({pct:F1}%) | components={e.Action.AttachedComponentCount} | rendered={e.Rendered} | drawn={e.Drawn} | meshes={e.Meshes} | vertices={e.Vertices}");
                }
                _logger.Info("=== End Pipeline Dump ===");
            }
        }
    }
}