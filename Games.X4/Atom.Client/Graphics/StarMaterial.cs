using FrameworkSDK.MonoGame.Graphics.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atom.Client.Graphics
{
    public class StarMaterial : IMeshMaterial
    {
        public Color StarColor { get; set; } = Color.White;
        public float Intensity { get; set; } = 1.0f;

        public StarMaterial(Color starColor, float intensity)
        {
            StarColor = starColor;
            Intensity = intensity;
        }

        public void ApplyToEffect(Effect effect)
        {
            var starEffect = (StarEffect)effect;
            starEffect.SetStarColor(StarColor);
            starEffect.SetIntensity(Intensity);
        }
    }
}
