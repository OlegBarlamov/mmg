using System;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.DrawableComponents;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace River.Client.MacOS
{
    [UsedImplicitly]
    public class RiverMapView : DrawablePrimitive<RiverMap>
    {
        public TilesResourcePackage TilesResourcePackage { get; }
        public IDisplayService DisplayService { get; }

        public RiverMapView(RiverMap data, [NotNull] TilesResourcePackage tilesResourcePackage, [NotNull] IDisplayService displayService)
            : base(data)
        {
            TilesResourcePackage = tilesResourcePackage ?? throw new ArgumentNullException(nameof(tilesResourcePackage));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
        }

        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);

            foreach (var mapTile in DataModel.Map)
            {
                if (mapTile.MapTileType == MapTileType.Empty) 
                    continue;

                var texture = TilesResourcePackage.GroundTexture;
                if (mapTile.MapTileType == MapTileType.Water)
                {
                    texture = TilesResourcePackage.WaterTexture;
                }

                var displayWidth = DisplayService.PreferredBackBufferWidth;
                var displayHeight = DisplayService.PreferredBackBufferHeight;
                
                context.Draw(texture, new Rectangle(
                        displayWidth / DataModel.Width * mapTile.MapPoint.X,
                        displayHeight - (displayHeight / DataModel.Height * mapTile.MapPoint.Y),
                        displayWidth / DataModel.Width,
                        displayHeight / DataModel.Height),
                    Color.White);
            }
        }
    }
}