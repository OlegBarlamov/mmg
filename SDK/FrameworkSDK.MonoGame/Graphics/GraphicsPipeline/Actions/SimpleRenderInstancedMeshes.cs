using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class RenderIdenticalMeshes<TVertexType> : GraphicsPipelineActionBase
        where TVertexType : struct, IVertexType
    {
        public Effect Effect { get; }
        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }

        private readonly Dictionary<Type, List<IRenderableMesh>> _meshesByGeometryType = new Dictionary<Type, List<IRenderableMesh>>();
        
        public RenderIdenticalMeshes([NotNull] string name,
            [NotNull] Effect effect, [NotNull] VertexBuffer vertexBuffer,
            [NotNull] IndexBuffer indexBuffer) : base(name)
        {
            Effect = effect ?? throw new ArgumentNullException(nameof(effect));
            VertexBuffer = vertexBuffer ?? throw new ArgumentNullException(nameof(vertexBuffer));
            IndexBuffer = indexBuffer ?? throw new ArgumentNullException(nameof(indexBuffer));
        }

        public override void OnComponentAttached(IGraphicComponent attachingComponent)
        {
            var meshes = attachingComponent.MeshesByPass[Name];
            foreach (var mesh in meshes)
            {
                var meshGeometryType = mesh.Geometry.GetType();
                if (!_meshesByGeometryType.ContainsKey(meshGeometryType))
                    _meshesByGeometryType.Add(meshGeometryType, new List<IRenderableMesh>());
            
                _meshesByGeometryType[meshGeometryType].Add(mesh);
            }
        }

        public override void OnComponentDetached(IGraphicComponent detachingComponent)
        {
            var meshes = detachingComponent.MeshesByPass[Name];
            foreach (var mesh in meshes)
            {
                var meshGeometryType = mesh.Geometry.GetType();
                _meshesByGeometryType[meshGeometryType].Remove(mesh);
            }
        }

        protected override void Dispose()
        {
            base.Dispose();
            
            _meshesByGeometryType.Clear();
        }

        private int _meshesIndex;
        private IRenderableMesh _mesh;

        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext,
            IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            graphicDeviceContext.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            graphicDeviceContext.GraphicsDevice.Indices = IndexBuffer;
            
            foreach (var meshesByGeometryName in _meshesByGeometryType)
            {
                var meshes = meshesByGeometryName.Value;
                var geometry = meshes[0].Geometry;
                
                VertexBuffer.SetData((TVertexType[]) geometry.GetVertices());
                IndexBuffer.SetData(geometry.GetIndices());

                for (_meshesIndex = 0; _meshesIndex < meshes.Count; _meshesIndex++)
                {
                    _mesh = meshes[_meshesIndex];
                    ((IEffectMatrices) Effect).World = _mesh.World;

                    foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        graphicDeviceContext.GraphicsDevice.DrawIndexedPrimitives(geometry.PrimitiveType, 0, 0,
                            geometry.GetPrimitivesCount());
                    }
                }
            }
        }
    }
}