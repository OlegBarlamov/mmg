using System;
using MonoGameExtensions.DataStructures;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.SceneComponents.Layout
{
    internal sealed class LayoutUiRoot : BaseLayoutUiContainer, ILayoutUiRoot
    {
        public event Action<ILayoutUiNode> NodeAdded;
        public event Action<ILayoutUiNode> NodeRemoved;

        public override bool IsVisual { get; protected set; } = false;
        
        public override void Invalidate(ILayoutUiNode child)
        {
            InvalidateElementsLayout(child);
        }

        protected override void OnChildAdded(ILayoutUiNode node)
        {
            base.OnChildAdded(node);
            node.Root = this;
            InvalidateElementsLayout(node);
            OnNodeAdded(node);
        }

        protected override void OnChildRemoved(ILayoutUiNode node)
        {
            base.OnChildRemoved(node);
            node.Root = null;
            OnNodeRemoved(node);
        }
        
        public void OnNodeAdded(ILayoutUiNode node)
        {
            node.Root = this;
            if (!node.IsAttachedToScene)
                NodeAdded?.Invoke(node);
            
            node.TraverseChildren(x =>
            {
                x.Root = this;
                if (!x.IsAttachedToScene)
                    NodeAdded?.Invoke(x);
            });
        }

        public void OnNodeRemoved(ILayoutUiNode node)
        {
            node.Root = null;
            if (node.IsAttachedToScene)
                NodeRemoved?.Invoke(node);
            
            node.TraverseChildren(x =>
            {
                x.Root = null;
                if (x.IsAttachedToScene)
                    NodeRemoved?.Invoke(x);
            });
        }

        public override bool CheckVisible()
        {
            return true;
        }

        public void InvalidateLayout(RectangleF rec)
        {
            ActualRect = rec;
            foreach (var element in Children)
            {
                InvalidateElementsLayout(element);
            }
        }
        

        private void InvalidateElementsLayout(ILayoutUiNode node)
        {
            node.Measure(ActualRect);
            node.Arrange(node.DesiredRect);
        }
    }
}