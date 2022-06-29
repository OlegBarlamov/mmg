using System;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Services
{
    public interface IIndicesBuffersFiller
    {
        void FillIndexBuffer(IndexBuffer indexBuffer, Array indicesArray);
    }
}