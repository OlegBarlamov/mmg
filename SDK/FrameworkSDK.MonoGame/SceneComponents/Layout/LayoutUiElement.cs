using FrameworkSDK.MonoGame.SceneComponents.Layout.Attributes;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.SceneComponents.Layout
{
    public class LayoutUiElement : BaseLayoutUiElement
    {
        public override bool IsVisual { get; protected set; } = true;

        // TODO Rethink the Measure Arrange functions. Doesn't work for StackPanel. Probably, Arrange should be called with the final rec of the child! 
        public override void Measure(RectangleF rect)
        {
            var finalSize = MeasureSelf(rect);
            if (Children.Count > 0)
            {
                var childrenSizeAppliedPadding = MeasureChildren(ApplyPadding(finalSize, Padding));
                var childrenSizeWithPadding = ApplyPadding(childrenSizeAppliedPadding, Padding.Inverse());
                finalSize = finalSize.GetMinimalContainingRectangle(childrenSizeWithPadding);
            }
            DesiredRect = finalSize;
        }

        public override void Arrange(RectangleF rect)
        {
            var finalSize = MeasureSelf(rect);
            if (Children.Count > 0)
            {
                var childrenSizeAppliedPadding = ArrangeChildren(ApplyPadding(finalSize, Padding));
                var childrenSizeWithPadding = ApplyPadding(childrenSizeAppliedPadding, Padding.Inverse());
                finalSize = finalSize.GetMinimalContainingRectangle(childrenSizeWithPadding);
            }
            ActualRect = finalSize;
        }

        protected virtual RectangleF MeasureChildren(RectangleF rect)
        {
            var finalRect = rect;
            foreach (var child in Children)
            {
                child.Measure(rect);
                finalRect = finalRect.GetMinimalContainingRectangle(child.DesiredRect);
            }
            return finalRect;
        }
        
        protected virtual RectangleF ArrangeChildren(RectangleF rect)
        {
            var finalRect = rect;
            foreach (var child in Children)
            {
                child.Arrange(rect);
                finalRect = finalRect.GetMinimalContainingRectangle(child.ActualRect);
            }
            return finalRect;
        }

        public override void Invalidate(ILayoutUiNode child)
        {
            Parent?.Invalidate(this);
        }

        protected override void OnChildAdded(ILayoutUiNode node)
        {
            base.OnChildAdded(node);
            
            Root?.OnNodeAdded(node);
            
            Parent?.Invalidate(this);
        }

        protected override void OnChildRemoved(ILayoutUiNode node)
        {
            base.OnChildRemoved(node);
            
            Root?.OnNodeRemoved(node);
            
            Parent?.Invalidate(this);
        }

        protected virtual RectangleF MeasureSelf(RectangleF rect)
        {
            var finalRect = new RectangleF();
            
            if (XPercentage != null)
                finalRect.X = rect.X + rect.Width * XPercentage.Value;
            if (YPercentage != null)
                finalRect.Y = rect.Y + rect.Height * YPercentage.Value;
            if (X != null)
                finalRect.X = rect.X + X.Value;
            if (Y != null)
                finalRect.Y = rect.Y + Y.Value;
            
            if (WidthPercentage != null)
                finalRect.Width = rect.Width * WidthPercentage.Value;
            if (HeightPercentage != null)
                finalRect.Height = rect.Height * HeightPercentage.Value;
            if (Width != null)
                finalRect.Width = Width.Value;
            if (Height != null)
                finalRect.Height = Height.Value;

            if (HorizontalAlignment == HorizontalAlignment.Left)
                finalRect.X = rect.X;
            if (HorizontalAlignment == HorizontalAlignment.Right)
                finalRect.X = rect.X + rect.Width - finalRect.Width;
            if (HorizontalAlignment == HorizontalAlignment.Center)
                finalRect.X = rect.X + rect.Width / 2 - finalRect.Width / 2;
            if (HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                finalRect.X = rect.X;
                finalRect.Width = rect.Width;
            }
            
            if (VerticalAlignment == VerticalAlignment.Top)
                finalRect.Y = rect.Y;
            if (VerticalAlignment == VerticalAlignment.Bottom)
                finalRect.Y = rect.Y + rect.Height - finalRect.Height;
            if (VerticalAlignment == VerticalAlignment.Center)
                finalRect.Y = rect.Y + rect.Height / 2 - finalRect.Height / 2;
            if (VerticalAlignment == VerticalAlignment.Stretch)
            {
                finalRect.Y = rect.Y;
                finalRect.Height = rect.Height;
            }

            finalRect.X += MarginX;
            finalRect.Y += MarginY;

            return finalRect;
        }

        private RectangleF ApplyPadding(RectangleF target, Padding padding)
        {
            return new RectangleF(
                target.X + padding.Left,
                target.Y + padding.Top,
                target.Width - padding.Left - padding.Right, 
                target.Height - padding.Top - padding.Bottom);
        }
    }
}