using System;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Map;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Services;
using FrameworkSDK.MonoGame.Services.Extensions;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;
using MonoGameExtensions.DataStructures;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Graphics.DrawableComponents
{
    public class TiledMapDrawableComponent : DrawablePrimitive<ViewModel<Tiled2DMap>>
    {
        public Rectangle DrawLocationOnScreen { get; set; }

        private ICamera2DProvider Camera2DProvider { get; }
        private IDisplayService DisplayService { get; }

        public TiledMapDrawableComponent(
            ViewModel<Tiled2DMap> viewModel,
            [NotNull] ICamera2DProvider camera2DProvider,
            [NotNull] IDisplayService displayService
            ) : base(viewModel)
        {
            Camera2DProvider = camera2DProvider ?? throw new ArgumentNullException(nameof(camera2DProvider));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));

            DrawLocationOnScreen = DisplayService.GetDisplayRectangle();
        }

        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);

            var camera = Camera2DProvider.GetActiveCamera();

            var worldRectangle = camera.FromDisplay(DrawLocationOnScreen);

            var mapRecStart = DataModel.Model.GetMapPointFromWorld(worldRectangle.Location)
                .Floor(Point.Zero, new Point(DataModel.Model.Width - 1, DataModel.Model.Height - 1));
            var mapRecEnd = DataModel.Model.GetMapPointFromWorld(worldRectangle.End)
                .Floor(Point.Zero, new Point(DataModel.Model.Width - 1, DataModel.Model.Height - 1));

            var displayCellSize = camera.ToDisplay(new RectangleF(0, 0, DataModel.Model.CellsSize.X, DataModel.Model.CellsSize.Y));
            
            for (int x = mapRecStart.X; x <= mapRecEnd.X; x++)
            {
                for (int y = mapRecStart.Y; y <= mapRecEnd.Y; y++)
                {
                    var cell = DataModel.Model.GetCell(x, y);
                    var rec = new RectangleF(
                        DrawLocationOnScreen.Left + x * displayCellSize.Width,
                        DrawLocationOnScreen.Top + y * displayCellSize.Height,
                        displayCellSize.Width,
                        displayCellSize.Height);
                    
                    cell.Visual.DrawTo(gameTime, rec, context);
                }
            }
        }
    }
}