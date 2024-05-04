using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.MonoGame.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omegas.Client.MacOs
{
    public class GameResourcePackage : ResourcePackage
    {
        public Texture2D Circle { get; private set; }
        
        public SpriteFont Font { get; private set; }

        public Texture2D SolidColor { get; private set; }

        public IReadOnlyList<Texture2D> MapBackgroundTexturesList => _mapBackgroundTexturesList;
        
        private List<Texture2D> _mapBackgroundTexturesList = new List<Texture2D>();
        
        protected override void Load(IContentLoaderApi content)
        {
            Circle = content.Primitives.Circle(100, Color.White);

            SolidColor = content.DiffuseColor(Color.White);
            
            // _mapBackgroundTexturesList = Enumerable.Range(0, 3)
            //     .Select(i => content.PointsNoise(128,128, 15, Color.White, Color.Black))
            //     .ToList();
            _mapBackgroundTexturesList = new List<Texture2D>
            {
                content.DiffuseColor(Color.Black)
            };

            Font = content.Load<SpriteFont>("Font");
        }
    }
}