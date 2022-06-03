using FrameworkSDK.Common;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Services
{
    public static class RandomServiceExtensions
    {
        public static Vector3 NextVector3(this IRandomService randomService, Vector3 minValue, Vector3 maxValue)
        {
            return new Vector3(randomService.NextFloat(minValue.X, maxValue.X), randomService.NextFloat(minValue.Y, maxValue.Y), randomService.NextFloat(minValue.Z, maxValue.Z));
        }
    }
}