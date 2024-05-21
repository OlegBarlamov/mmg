using System.Collections.Generic;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.DrawableComponents.Stencils;
using FrameworkSDK.MonoGame.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetExtensions.Collections;

namespace Omegas.Client.MacOs.Models
{
    public class OmegaTiledMap : Tiled2DMap
    {
        public OmegaTiledMap(GameResourcePackage gameResourcePackage) 
            : base(GetData(gameResourcePackage.MapBackgroundTexturesList), new Vector2(256))
        {
            
        }

        private static Tiled2DCell[,] GetData(IReadOnlyList<Texture2D> texturesList)
        {
            var mapData = new Tiled2DCell[4, 4];
            mapData.Fill((x, y) =>
            {
                var texture = texturesList.PickRandom();
                return new Tiled2DCell(new Point(x, y), new TextureStencil(texture, Color.White));
            });
            return mapData;
        }
    }
}