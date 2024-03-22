using System;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.DrawableComponents;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class BackgroundTextureComponentDataModel : ViewModel
    {
        public Texture2D Texture { get; set; }
    }
    
    public class BackgroundTextureComponent : DrawablePrimitive<BackgroundTextureComponentDataModel>
    {
        private IDisplayService DisplayService { get; }

        // ReSharper disable once UnusedParameter.Local
        public BackgroundTextureComponent([NotNull] BackgroundTextureComponentDataModel dataModel, [NotNull] IDisplayService displayService)
            : base(dataModel)
        {
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
        }
        
        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            context.Draw(DataModel.Texture, DisplayService.GraphicsDevice.Viewport.Bounds.ToRectangleF(), Color.White);
            
            base.Draw(gameTime, context);
        }
    }
}