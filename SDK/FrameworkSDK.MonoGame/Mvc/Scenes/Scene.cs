using System;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.Graphics.Services;
using FrameworkSDK.MonoGame.SceneComponents.Layout;
using FrameworkSDK.MonoGame.Services;
using FrameworkSDK.MonoGame.Services.Implementations;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;

namespace FrameworkSDK.MonoGame.Mvc
{
    public abstract class Scene : SceneBase
    {
        public ILayoutUiContainer UI => _layoutSystem.LayoutUiRoot;
        
        [NotNull] protected IRenderTargetsFactoryService RenderTargetsFactoryService { get; }
        [NotNull] protected IGameHeartServices GameHeartServices { get; }

        protected sealed override bool IsInitialized => _isInitialized;

        protected bool IsDisposed { get; private set; }

        [NotNull] private IGraphicsPipelineFactoryService GraphicsPipelineFactoryService { get; }
        
        [NotNull] private ModuleLogger Logger { get; } = new ModuleLogger(LogCategories.Mvc);

        [CanBeNull] private IGraphicsPipeline _usedGraphicsPipeline;

        private bool _isInitialized;
        
        private readonly SceneLayout _layoutSystem;

        protected Scene([NotNull] string name, object model = null)
            : base(name, model)
        {
            GraphicsPipelineFactoryService = AppContext.ServiceLocator.Resolve<IGraphicsPipelineFactoryService>();
            RenderTargetsFactoryService = AppContext.ServiceLocator.Resolve<IRenderTargetsFactoryService>();
            GameHeartServices = AppContext.ServiceLocator.Resolve<IGameHeartServices>();

            _layoutSystem = new SceneLayout(AppContext.ServiceLocator.Resolve<IDisplayService>(), this);
            AddExtension(_layoutSystem);
            
            ProcessDelayedInitialization();
        }

        private void ProcessDelayedInitialization()
        {
            var appStateService = AppContext.ServiceLocator.Resolve<AppStateService>();
            if (appStateService.CoreResourceLoaded)
                appStateService.QueueOnUpdate(SafeInitialization);
            else
                appStateService.QueueOnAppReady(SafeInitialization);
        }

        private void SafeInitialization(GameTime gameTime)
        {
            SafeInitialization();
        }
        
        private void SafeInitialization()
        {
            if (IsDisposed)
                return;
            
            try
            {
                _usedGraphicsPipeline = BuildGraphicsPipeline(GraphicsPipelineFactoryService.Create(GraphicComponents));
                _isInitialized = true;
                
                Initialize();
            }
            catch (Exception e)
            {
                Logger.Error($"Scene '{Name}' initialization failed", e);
            }
        }

        protected Scene(object model)
            : this(GenerateSceneName(), model)
        {

        }

        protected Scene()
            :this(GenerateSceneName())
        {
        }

        public override void Dispose()
        {
            if (IsDisposed) throw new ObjectDisposedException($"Scene {Name}");
            IsDisposed = true;
            
            base.Dispose();
            
            _layoutSystem.Dispose();
            _usedGraphicsPipeline?.Dispose();
        }

        protected sealed override IGraphicsPipeline GetGraphicsPipeline()
        {
            return _usedGraphicsPipeline ?? throw new FrameworkMonoGameException($"Used graphics pipeline must be specified in Scene: {Name}");
        }

        protected abstract void Initialize();

        [NotNull]
        protected abstract IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder);
    }
}