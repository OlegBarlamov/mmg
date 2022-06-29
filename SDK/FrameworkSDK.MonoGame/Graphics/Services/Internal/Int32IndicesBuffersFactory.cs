using System;
using System.Linq;
using FrameworkSDK.MonoGame.Core;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Services
{
    internal class Int32IndicesBuffersFactory : IIndicesBuffersFactory
    {
        private IGameHeartServices GameHeartServices { get; }
        private GraphicsDevice GraphicsDevice => GameHeartServices.GraphicsDeviceManager.GraphicsDevice;

        public Int32IndicesBuffersFactory([NotNull] IGameHeartServices gameHeartServices)
        {
            GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
        }
        
        public IndexBuffer CreateIndexBuffer(int indicesCount)
        {
            return new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, indicesCount, BufferUsage.WriteOnly);
        }

        public Array CreateIndicesArray([NotNull] params int[] indices)
        {
            return indices ?? throw new ArgumentNullException(nameof(indices));
        }

        public Array CreateShortIndicesArray([NotNull] params short[] indices)
        {
            if (indices == null) throw new ArgumentNullException(nameof(indices));
            return ConvertShortToInt(indices);
        }

        private int[] ConvertShortToInt([NotNull] short[] indices)
        {
            return indices.Select(x => (int) x).ToArray();
        }
    }
}