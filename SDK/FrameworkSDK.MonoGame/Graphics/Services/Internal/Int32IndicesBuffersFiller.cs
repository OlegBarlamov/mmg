using System;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Services
{
    internal class Int32IndicesBuffersFiller : IIndicesBuffersFiller
    {
        public void FillIndexBuffer(IndexBuffer indexBuffer, Array indicesArray)
        {
            indexBuffer.SetData((int[])indicesArray);
        }
    }
}