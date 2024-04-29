using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using Omegas.Client.MacOs.Models.SphereObject;

namespace Omegas.Client.MacOs.Models
{
    public class PlayerViewModel : SphereObjectViewModel
    {
        public RectangleF HeartBoundingBox { get; set; }

        public Color HeartColor { get; set; }
    }
}