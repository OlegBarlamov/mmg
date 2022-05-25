using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.Graphics.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Mvc
{
    public abstract class Scene : SceneBase
    {
        [NotNull] private IGraphicsPipelineFactoryService GraphicsPipelineFactoryService { get; }
        [NotNull] private IRenderTargetsFactoryService RenderTargetsFactoryService { get; }

        private bool _initialized;
        
        [CanBeNull] private IGraphicsPipeline _usedGraphicsPipeline;
        [CanBeNull] private IRenderTargetWrapper _defaultGraphicsPipelineRenderTarget;

        protected Scene([NotNull] string name, object model = null)
            : base(name, model)
        {
            GraphicsPipelineFactoryService = AppContext.ServiceLocator.Resolve<IGraphicsPipelineFactoryService>();
            RenderTargetsFactoryService = AppContext.ServiceLocator.Resolve<IRenderTargetsFactoryService>();
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
            base.Dispose();
            
            _usedGraphicsPipeline?.Dispose();
            _defaultGraphicsPipelineRenderTarget?.Dispose();
        }

        protected override IGraphicsPipeline GetGraphicsPipeline()
        {
            return _usedGraphicsPipeline ?? throw new FrameworkMonoGameException($"Used graphics pipeline must be specified in Scene: {Name}");
        }

        protected override void OnOpening()
        {
            base.OnOpening();
            
            if (!_initialized)
            {
                _initialized = true;
                OnFirstOpening();
            }
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            
            if (_usedGraphicsPipeline == null)
            {
                _usedGraphicsPipeline = BuildGraphicsPipeline(GraphicsPipelineFactoryService.Create(GraphicComponents));
            }

        }

        private IGraphicsPipeline CreateDefaultPipeline(IGraphicsPipelineBuilder builder)
        {
            _defaultGraphicsPipelineRenderTarget = RenderTargetsFactoryService.CreateFullScreenRenderTarget(
                false,
                SurfaceFormat.Color,
                DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            return builder
                .SetRenderTarget(_defaultGraphicsPipelineRenderTarget.RenderTarget)
                .BeginDraw(new BeginDrawConfig())
                .DrawComponents()
                .EndDraw()
                .DrawRenderTargetToDisplay(_defaultGraphicsPipelineRenderTarget.RenderTarget)
                .Build();
        }

        [NotNull] protected virtual IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder)
        {
            return CreateDefaultPipeline(graphicsPipelineBuilder);
        }
    }
}