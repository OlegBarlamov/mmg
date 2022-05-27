using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.Graphics.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Mvc
{
    public abstract class Scene : SceneBase
    {
        public static class DefaultPipelineActions
        {
            public const string Draw = "Default";
            public const string RenderVertexPosition = "Render_VP";
            public const string RenderVertexPositionColor = "Render_VPC";
            public const string RenderVertexPositionTexture = "Render_VPT";
        }
        
        [NotNull] protected IRenderTargetsFactoryService RenderTargetsFactoryService { get; }
        [NotNull] protected IGameHeartServices GameHeartServices { get; }

        [NotNull] private IGraphicsPipelineFactoryService GraphicsPipelineFactoryService { get; }

        [CanBeNull] private IGraphicsPipeline _usedGraphicsPipeline;
        [CanBeNull] private IRenderTargetWrapper _defaultGraphicsPipelineRenderTarget;
        
        private bool _initialized;

        protected Scene([NotNull] string name, object model = null)
            : base(name, model)
        {
            GraphicsPipelineFactoryService = AppContext.ServiceLocator.Resolve<IGraphicsPipelineFactoryService>();
            RenderTargetsFactoryService = AppContext.ServiceLocator.Resolve<IRenderTargetsFactoryService>();
            GameHeartServices = AppContext.ServiceLocator.Resolve<IGameHeartServices>();
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
                .DrawComponents(DefaultPipelineActions.Draw)
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