using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Resources.Generation
{
    internal interface ITextureGeneratorService
    {
        Texture2D DiffuseColor(Color color);

        Texture2D GradientColor(Color color1, Color color2, int width, int height, float angle,
            float offset = 0);
    }
}