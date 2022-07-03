using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public abstract class GraphicsPipelineActionBase : IGraphicsPipelineAction
    {
        public string Name { get; }

        public bool IsDisabled { get; set; }

        internal const string DebugInfoRenderingComponents = "render_comps";
        internal const string DebugInfoRenderingMeshes = "render_meshes";
        internal const string DebugInfoRenderingVertices = "render_vertices";
        internal const string DebugInfoDrawComponents = "draw_comps";
        
        protected List<IGraphicComponent> AttachedComponents { get; } = new List<IGraphicComponent>();
        
        protected GraphicsPipelineActionBase([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        
        public void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext)
        {
            Process(gameTime, graphicDeviceContext, AttachedComponents);
        }

        public virtual void OnComponentAttached(IGraphicComponent attachingComponent)
        {
            AttachedComponents.Add(attachingComponent);
        }

        public virtual void OnComponentDetached(IGraphicComponent detachingComponent)
        {
            AttachedComponents.Remove(detachingComponent);
        }

        void IDisposable.Dispose()
        {
            Dispose();
            AttachedComponents.Clear();
        }

        public abstract void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext,
            IReadOnlyList<IGraphicComponent> associatedComponents);

        protected virtual void Dispose()
        {
            
        }

        protected static bool IsComponentVisibleByActiveCamera(IGraphicDeviceContext graphicDeviceContext, IGraphicComponent component)
        {
            return component.BoundingBox == null || graphicDeviceContext.Camera3DProvider.GetActiveCamera()
                .CheckBoundingBoxVisible(component.BoundingBox.Value);
        }
    }
}