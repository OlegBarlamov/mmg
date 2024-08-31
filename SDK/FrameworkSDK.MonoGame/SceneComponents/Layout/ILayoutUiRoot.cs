using System;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.SceneComponents.Layout
{
    public interface ILayoutUiRoot : ILayoutUiContainer
    {
        event Action<ILayoutUiNode> NodeAdded;
        event Action<ILayoutUiNode> NodeRemoved;
        void OnNodeAdded(ILayoutUiNode node);
        void OnNodeRemoved(ILayoutUiNode node);
        void InvalidateLayout(RectangleF rect);
    }
}