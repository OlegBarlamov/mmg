using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;

namespace Omegas.Client.MacOs.Models
{
    public class CharacterViewModel
    {
        public RectangleF BoundingBox { get; set; }
        
        public RectangleF HeartBoundingBox { get; set; }
        
        public Color Color { get; set; }
        
        public Color HeartColor { get; set; }
    }
}