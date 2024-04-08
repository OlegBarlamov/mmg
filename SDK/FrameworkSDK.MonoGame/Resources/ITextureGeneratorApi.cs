using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Resources
{
    public interface ITextureGeneratorApi
    {
        Texture2D EmptyTexture(int width, int height);
        Texture2D DiffuseColor(Color color);

        Texture2D GradientColor(Color color1, Color color2, int width, int height, float angleDegrees,
            float offset = 0);

        Texture2D HeightMap(int[,] heights, int minValue, int maxValue, Color minValueColor, Color maxValueColor);
        
        ITexturePrimitivesGenerator Primitives { get; }

        Texture2D PointsNoise(int width, int height, int pointsCount, Color pointsColor, Color backgroundColor);
    }
}