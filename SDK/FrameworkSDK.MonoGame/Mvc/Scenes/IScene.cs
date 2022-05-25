using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Mvc
{
	internal interface IScene : IControllersManager, IViewsManager, IUpdatable, IClosable, INamed
    {
        [NotNull] IGraphicsPipeline GraphicsPipeline { get; }
        
        object DataModel { get; set; }

        void OnOpened();
        void OnOpening();
    }
}
