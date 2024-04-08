using System.Drawing;
using FrameworkSDK.Common;

namespace FrameworkSDK.Services.Randoms
{
    public static class RandomServiceExtensions
    {
        public static Point NextPoint(this IRandomService randomService, Point minPoint, Point maxPoint)
        {
            var x = randomService.NextInteger(minPoint.X, maxPoint.X);
            var y = randomService.NextInteger(minPoint.Y, maxPoint.Y);
            return new Point(x, y);
        }
    }
}