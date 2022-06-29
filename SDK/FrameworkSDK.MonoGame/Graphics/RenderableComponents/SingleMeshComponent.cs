using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.RenderableComponents
{
    public abstract class SingleMeshComponent<TData, TController> : RenderableView<TData, TController> where TController : IController
    {
        public override IReadOnlyList<string> GraphicsPassNames { get; }
        public override IReadOnlyDictionary<string, IReadOnlyList<IRenderableMesh>> MeshesByPass { get; }
        
        public override BoundingBox? BoundingBox => MeshBoundingBox;
        
        protected abstract BoundingBox MeshBoundingBox { get; }

        protected IRenderableMesh Mesh { get; }

        protected SingleMeshComponent(IRenderableMesh mesh, string graphicsPassName)
        {
            Mesh = mesh;
            Mesh.Parent = this;
            GraphicsPassNames = new[] {graphicsPassName};
            MeshesByPass = new Dictionary<string, IReadOnlyList<IRenderableMesh>>()
            {
                {graphicsPassName, new[] {Mesh}}
            };
        }

    }

    public abstract class SingleMeshComponent<TData> : SingleMeshComponent<TData, EmptyController>
    {
        protected SingleMeshComponent(IRenderableMesh mesh, string graphicsPassName) : base(mesh, graphicsPassName)
        {
        }
    }
}