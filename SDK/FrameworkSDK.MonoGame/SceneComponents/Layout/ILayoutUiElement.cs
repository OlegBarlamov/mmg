using FrameworkSDK.MonoGame.SceneComponents.Layout.Attributes;

namespace FrameworkSDK.MonoGame.SceneComponents.Layout
{
    public interface ILayoutUiElement : ILayoutUiNode
    {
        float? X { get; }
        float? Y { get; }
        float? Width { get; }
        float? Height { get; }
        
        float? XPercentage { get; }
        float? YPercentage { get; }
        float? WidthPercentage { get; }
        float? HeightPercentage { get; }
        
        HorizontalAlignment HorizontalAlignment { get; }
        VerticalAlignment VerticalAlignment { get; }
        
        float MarginX { get; }
        float MarginY { get; }
        
        bool IsVisible { get; }

        Padding Padding { get; }
    }
}