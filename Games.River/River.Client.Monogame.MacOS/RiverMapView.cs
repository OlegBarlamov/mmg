using System;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.DrawableComponents;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using MonoGameExtensions.Geometry;

namespace River.Client.MacOS
{
    [UsedImplicitly]
    public class RiverMapView : DrawablePrimitive<RiverMap>
    {
        public TilesResourcePackage TilesResourcePackage { get; }
        public ICamera2DProvider Camera2DProvider { get; }

        public RiverMapView(RiverMap data, [NotNull] TilesResourcePackage tilesResourcePackage,
            [NotNull] ICamera2DProvider camera2DProvider)
            : base(data)
        {
            TilesResourcePackage = tilesResourcePackage ?? throw new ArgumentNullException(nameof(tilesResourcePackage));
            Camera2DProvider = camera2DProvider ?? throw new ArgumentNullException(nameof(camera2DProvider));
        }

        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);

            var camera = Camera2DProvider.GetActiveCamera();
            var cameraViewport = camera.Viewport();
            foreach (var mapTile in DataModel.Map.GetInRectangleBounded(cameraViewport))
            {
                if (mapTile.MapTileType == MapTileType.Empty)
                    continue;

                var texture = TilesResourcePackage.GroundTexture;
                if (mapTile.MapTileType == MapTileType.Water)
                {
                    texture = TilesResourcePackage.WaterTexture;
                }
                
                var tile = new RectangleF(mapTile.MapPoint.ToVector2(), new Vector2(32));
                context.Draw(texture, tile, Color.White);
            }
        }
    }
}