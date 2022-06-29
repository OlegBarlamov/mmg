using System;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Graphics.Services;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Meshes
{
    public abstract class MeshGeometryBase<TVertexType> : IMeshGeometry where TVertexType : struct, IVertexType
    {
        public PrimitiveType PrimitiveType { get; }
        public VertexDeclaration VertexDeclaration { get; }

        protected IIndicesBuffersFactory IndicesBuffersFactory { get; } =
            AppContext.ServiceLocator.Resolve<IIndicesBuffersFactory>();

        protected MeshGeometryBase(VertexDeclaration vertexDeclaration, PrimitiveType primitiveType)
        {
            PrimitiveType = primitiveType;
            VertexDeclaration = vertexDeclaration;
        }
        
        Array IMeshGeometry.GetVertices()
        {
            return GetVertices();
        }

        Array IMeshGeometry.GetIndices()
        {
            return GetIndices();
        }

        public abstract int GetPrimitivesCount();

        protected abstract TVertexType[] GetVertices();

        protected abstract Array GetIndices();
        
        void IMeshGeometry.FillVertexBuffer(VertexBuffer vertexBuffer)
        {
            vertexBuffer.SetData(GetVertices());
        }
    }
}