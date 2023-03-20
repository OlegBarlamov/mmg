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
        [NotNull] protected IRenderTargetsFactoryService RenderTargetsFactoryService { get; }
        [NotNull] protected IGameHeartServices GameHeartServices { get; }

        [NotNull] private IGraphicsPipelineFactoryService GraphicsPipelineFactoryService { get; }

        [CanBeNull] private IGraphicsPipeline _usedGraphicsPipeline;

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
        }

        protected sealed override IGraphicsPipeline GetGraphicsPipeline()
        {
            return _usedGraphicsPipeline ?? throw new FrameworkMonoGameException($"Used graphics pipeline must be specified in Scene: {Name}");
        }

        protected override void OnOpening()
        {
            base.OnOpening();
            
            if (!_initialized)
            {
                _initialized = true;
                _usedGraphicsPipeline = BuildGraphicsPipeline(GraphicsPipelineFactoryService.Create(GraphicComponents));
                OnFirstOpening();
            }
        }

        [NotNull]
        protected abstract IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder);
    }
}