using MonoGameExtensions.DataStructures;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.SceneComponents.Layout
{
    public interface ILayoutUiNode : ITreeNode<ILayoutUiNode, ILayoutUiContainer>
    {
        bool IsVisual { get; }
        bool IsAttachedToScene { get; set; }
        ILayoutUiRoot Root { get; set; }
        RectangleF DesiredRect { get; }
        RectangleF ActualRect { get; }
        void Measure(RectangleF rect);
        void Arrange(RectangleF rect);
    }
}