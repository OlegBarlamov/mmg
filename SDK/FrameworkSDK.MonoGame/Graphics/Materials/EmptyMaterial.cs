using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Materials
{
    public class EmptyMaterial : IMeshMaterial
    {
        public void ApplyToEffect(Effect effect)
        {
            // Nothing
        }
    }
}