using System;
using System.Linq;
using FrameworkSDK.MonoGame.Core;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Services
{
    internal class Int16IndicesBuffersFactory : IIndicesBuffersFactory
    {
        private IGameHeartServices GameHeartServices { get; }
        private GraphicsDevice GraphicsDevice => GameHeartServices.GraphicsDeviceManager.GraphicsDevice;

        public Int16IndicesBuffersFactory([NotNull] IGameHeartServices gameHeartServices)
        {
            GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
        }
        
        public IndexBuffer CreateIndexBuffer(int indicesCount)
        {
            return new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, indicesCount, BufferUsage.WriteOnly);
        }

        public Array CreateIndicesArray([NotNull] params int[] indices)
        {
            if (indices == null) throw new ArgumentNullException(nameof(indices));
            return ConvertIntToShort(indices);
        }

        public Array CreateShortIndicesArray([NotNull] params short[] indices)
        {
            return indices ?? throw new ArgumentNullException(nameof(indices));
        }

        private short[] ConvertIntToShort([NotNull] int[] indices)
        {
            return indices.Select(x => (short) x).ToArray();
        }
    }
}