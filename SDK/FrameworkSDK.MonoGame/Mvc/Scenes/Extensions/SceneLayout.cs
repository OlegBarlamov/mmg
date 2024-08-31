using System;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.SceneComponents.Layout;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Mvc
{
    public class SceneLayout : ISceneExtension, IDisposable
    {
        public ILayoutUiRoot LayoutUiRoot => _uiRoot;
        [NotNull] private IDisplayService DisplayService { get; }
        [NotNull] private IViewsManager ViewsManager { get; }
        
        private readonly LayoutUiRoot _uiRoot;

        public string Name => "UI";

        public SceneLayout([NotNull] IDisplayService displayService, [NotNull] IViewsManager viewsManager)
        {
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            ViewsManager = viewsManager ?? throw new ArgumentNullException(nameof(viewsManager));
            _uiRoot = new LayoutUiRoot();
            _uiRoot.NodeAdded += UiRootOnNodeAdded;
            _uiRoot.NodeRemoved += UiRootOnNodeRemoved;
            DisplayServiceOnDeviceReset();
        }

        public void Dispose()
        {
            DisplayService.DeviceReset -= DisplayServiceOnDeviceReset;
            _uiRoot.NodeAdded -= UiRootOnNodeAdded;
            _uiRoot.NodeRemoved -= UiRootOnNodeRemoved;
        }
        
        public void OnViewAttached(IView view)
        {
            if (view.DataModel is ILayoutUiNode node)
            {
                node.IsAttachedToScene = true;
                // Node added directly from the Scene API
                if (node.Parent == null)
                    _uiRoot.AddChild(node);
            }
        }

        public void OnViewDetached(IView view)
        {
            if (view.DataModel is ILayoutUiNode node)
            {
                node.IsAttachedToScene = false;
                // Node removed directly from the Scene API
                if (node.Parent != null)
                    node.Parent.RemoveChild(node);
            }
        }
        
        private void UiRootOnNodeAdded(ILayoutUiNode node)
        {
            node.IsAttachedToScene = true;
            if (node.IsVisual) 
                ViewsManager.AddView(node);
        }
        
        private void UiRootOnNodeRemoved(ILayoutUiNode node)
        {
            node.IsAttachedToScene = false;
            if (node.IsVisual)
                ViewsManager.RemoveView(node);
        }

        private void DisplayServiceOnDeviceReset()
        {
            var width = DisplayService.PreferredBackBufferWidth;
            var height = DisplayService.PreferredBackBufferHeight;
            var rec = new RectangleF(0, 0, width, height);
            _uiRoot.InvalidateLayout(rec);
        }
        
        void ISceneExtension.OnClosed()
        {
            DisplayService.DeviceReset -= DisplayServiceOnDeviceReset;
        }

        void ISceneExtension.OnOpened()
        {
            DisplayService.DeviceReset += DisplayServiceOnDeviceReset;
        }

        void IUpdatable.Update(GameTime gameTime)
        {
        }

        void ISceneExtension.OnOpening()
        {
        }

        void ISceneExtension.OnControllerAttached(IController controller)
        {
        }

        void ISceneExtension.OnControllerDetached(IController controller)
        {
        }
    }
}