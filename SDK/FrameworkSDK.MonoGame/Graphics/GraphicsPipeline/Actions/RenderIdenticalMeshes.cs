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
        public IRenderableMesh Mesh { get; }

        public RenderIdenticalMeshes([NotNull] string name,
            [NotNull] Effect effect, [NotNull] VertexBuffer vertexBuffer,
            [NotNull] IndexBuffer indexBuffer, [NotNull] IRenderableMesh mesh) : base(name)
        {
            Effect = effect ?? throw new ArgumentNullException(nameof(effect));
            VertexBuffer = vertexBuffer ?? throw new ArgumentNullException(nameof(vertexBuffer));
            IndexBuffer = indexBuffer ?? throw new ArgumentNullException(nameof(indexBuffer));
            Mesh = mesh ?? throw new ArgumentNullException(nameof(mesh));
            
            VertexBuffer.SetData((TVertexType[]) Mesh.GetVertices());
            IndexBuffer.SetData(Mesh.GetIndices());
        }
        
        private int _index;
        private int _meshesIndex;
        private IReadOnlyList<IRenderableMesh> _meshes;
        private IRenderableMesh _mesh;

        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext,
            IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            graphicDeviceContext.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            graphicDeviceContext.GraphicsDevice.Indices = IndexBuffer;
            
            for (_index = 0; _index < associatedComponents.Count; _index++)
            {
                _meshes = associatedComponents[_index].MeshesByPass[Name];

                for (_meshesIndex = 0; _meshesIndex < _meshes.Count; _meshesIndex++)
                {
                    foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                    {
                        _mesh = _meshes[_meshesIndex];

                        ((IEffectMatrices) Effect).World = _mesh.World;
                        pass.Apply();

                        graphicDeviceContext.GraphicsDevice.DrawIndexedPrimitives(Mesh.PrimitiveType, 0, 0,
                            Mesh.GetPrimitivesCount());
                    }
                }
            }
        }
    }
}