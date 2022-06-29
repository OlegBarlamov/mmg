using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.RenderingTools
{
    public static class RasterizerStates
    {
        public static RasterizerState Default { get; } = new RasterizerState 
        {
            FillMode = FillMode.Solid,
            CullMode = CullMode.CullCounterClockwiseFace,
        };
        
        public static RasterizerState WireFrame { get; } = new RasterizerState 
        {
            FillMode = FillMode.WireFrame,
            CullMode = CullMode.CullCounterClockwiseFace,
        };
    }
}