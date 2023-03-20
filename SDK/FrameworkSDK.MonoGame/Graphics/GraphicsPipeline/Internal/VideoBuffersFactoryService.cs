using System;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Graphics.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class VideoBuffersFactoryService : IVideoBuffersFactoryService
    {
        [NotNull] private IIndicesBuffersFactory IndicesBuffersFactory { get; }
        [NotNull] private IGameHeartServices GameHeartServices { get; }

        public VideoBuffersFactoryService([NotNull] IIndicesBuffersFactory indicesBuffersFactory, [NotNull] IGameHeartServices gameHeartServices)
        {
            IndicesBuffersFactory = indicesBuffersFactory ?? throw new ArgumentNullException(nameof(indicesBuffersFactory));
            GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
        }
        
        public VertexBuffer CreateVertexBugger(VertexDeclaration vertexDeclaration, int vertexCount)
        {
            return new VertexBuffer(GameHeartServices.GraphicsDeviceManager.GraphicsDevice, vertexDeclaration, vertexCount, BufferUsage.WriteOnly);
        }

        public IndexBuffer CreateIndexBuffer(int indicesCount)
        {
            return IndicesBuffersFactory.CreateIndexBuffer(indicesCount);
        }
    }
}