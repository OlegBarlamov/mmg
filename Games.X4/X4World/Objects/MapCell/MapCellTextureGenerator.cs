using System.Threading;
using System.Threading.Tasks;
using FrameworkSDK.MonoGame.Resources.Generation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace X4World.Objects
{
    public static class MapCellTextureGenerator
    {
        public static Texture2D GenerateAsync(Vector3 cameraPosition, byte[,,] data, ITextureGeneratorService textureGeneratorService, CancellationToken cancellationToken)
        {
            return textureGeneratorService.DiffuseColor(Color.Purple);
        }
    }
}