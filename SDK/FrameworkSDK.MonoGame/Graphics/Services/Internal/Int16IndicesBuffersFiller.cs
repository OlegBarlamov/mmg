using System;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Services
{
    public class Int16IndicesBuffersFiller : IIndicesBuffersFiller
    {
        public void FillIndexBuffer(IndexBuffer indexBuffer, Array indicesArray)
        {
            indexBuffer.SetData((short[])indicesArray);
        }
    }
}