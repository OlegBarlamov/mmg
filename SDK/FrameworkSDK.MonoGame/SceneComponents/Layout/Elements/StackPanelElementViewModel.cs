using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.SceneComponents.Layout.Elements
{
    public class StackPanelElementViewModel : LayoutUiElement
    {

        public enum StackPanelOrientation
        {
            Horizontal,
            Vertical
        }

        public override bool IsVisual { get; protected set; } = false;

        public StackPanelOrientation Orientation { get; set; } = StackPanelOrientation.Horizontal;

        public StackPanelElementViewModel(params ILayoutUiNode[] children)
        {
            foreach (var node in children)
            {
                AddChild(node);
            }
        }

        protected override RectangleF MeasureChildren(RectangleF rect)
        {
            if (Orientation == StackPanelOrientation.Horizontal)
                return MeasureChildrenHorizontal(rect);
            else 
                return MeasureChildrenVertical(rect);
        }

        protected override RectangleF ArrangeChildren(RectangleF rect)
        {
            if (Orientation == StackPanelOrientation.Horizontal)
                return ArrangeChildrenHorizontal(rect);
            else 
                return ArrangeChildrenVertical(rect);
        }

        private RectangleF MeasureChildrenHorizontal(RectangleF rect)
        {
            var finalRect = rect;
            var x = rect.Left;
            foreach (var child in Children)
            {
                var childRect = new RectangleF(x, rect.Y, rect.Width - (x - rect.Left), rect.Height);
                child.Measure(childRect);
                finalRect = finalRect.GetMinimalContainingRectangle(child.DesiredRect);
                x = child.DesiredRect.Right;
            }
            return finalRect;
        }
        
        private RectangleF MeasureChildrenVertical(RectangleF rect)
        {
            var finalRect = rect;
            var y = rect.Top;
            foreach (var child in Children)
            {
                var childRect = new RectangleF(rect.X, y, rect.Width, rect.Height - (y - rect.Top));
                child.Measure(childRect);
                finalRect = finalRect.GetMinimalContainingRectangle(child.DesiredRect);
                y = child.DesiredRect.Right;
            }
            return finalRect;
        }
        
        private RectangleF ArrangeChildrenHorizontal(RectangleF rect)
        {
            var finalRect = rect;
            var x = rect.Left;
            foreach (var child in Children)
            {
                var childRect = new RectangleF(x, rect.Y, rect.Width - (x - rect.Left), rect.Height);
                child.Arrange(childRect);
                finalRect = finalRect.GetMinimalContainingRectangle(child.DesiredRect);
                x = child.DesiredRect.Right;
            }
            return finalRect;
        }
        
        private RectangleF ArrangeChildrenVertical(RectangleF rect)
        {
            var finalRect = rect;
            var y = rect.Top;
            foreach (var child in Children)
            {
                var childRect = new RectangleF(rect.X, y, rect.Width, rect.Height - (y - rect.Top));
                child.Arrange(childRect);
                finalRect = finalRect.GetMinimalContainingRectangle(child.DesiredRect);
                y = child.DesiredRect.Right;
            }
            return finalRect;
        }
    }
}