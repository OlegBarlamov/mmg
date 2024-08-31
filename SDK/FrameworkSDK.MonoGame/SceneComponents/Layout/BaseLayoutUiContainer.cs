using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using JetBrains.Annotations;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.SceneComponents.Layout
{
    public abstract class BaseLayoutUiContainer : ViewModel, ILayoutUiContainer
    {
        public abstract bool IsVisual { get; protected set; }
        public bool IsAttachedToScene { get; set; }
        [CanBeNull] public ILayoutUiContainer Parent { get; set; }
        [CanBeNull] public ILayoutUiRoot Root { get; set; }
        public RectangleF DesiredRect { get; protected set; }
        public RectangleF ActualRect { get; protected set; }
        public IReadOnlyCollection<ILayoutUiNode> Children => _children;

        private readonly List<ILayoutUiNode> _children = new List<ILayoutUiNode>();

        public ILayoutUiContainer AddChild(ILayoutUiNode node)
        {
            if (node.Parent != null)
                throw new ArgumentException($"The node {node} is already attached to the parent {node.Parent}");
                
            node.Parent = this;
            _children.Add(node);
            OnChildAdded(node);
            return this;
        }

        public ILayoutUiContainer RemoveChild(ILayoutUiNode node)
        {
            if (_children.Remove(node))
            {
                node.Parent = null;
                OnChildRemoved(node);
            }
            return this;
        }

        public abstract bool CheckVisible();

        public abstract void Invalidate(ILayoutUiNode child);

        protected virtual void OnChildAdded(ILayoutUiNode node)
        {
            
        }

        protected virtual void OnChildRemoved(ILayoutUiNode node)
        {
            
        }
        
        public virtual void Measure(RectangleF rect)
        {
        }

        public virtual void Arrange(RectangleF rect)
        {
        }
    }
}