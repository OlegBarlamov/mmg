using System;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Services
{
    public interface IIndicesBuffersFactory
    {
        IndexBuffer CreateIndexBuffer(int indicesCount);

        Array CreateIndicesArray(params int[] indices);
        Array CreateShortIndicesArray(params short[] indices);
    }
}