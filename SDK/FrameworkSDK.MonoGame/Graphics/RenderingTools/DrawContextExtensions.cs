using System;
using FrameworkSDK.MonoGame.Graphics.Basic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.RenderingTools
{
    public static class DrawContextExtensions
    {
        public static void Draw([NotNull] this IDrawContext drawContext, [NotNull] Texture2D texture,
            [NotNull] DrawParameters drawParameters)
        {
            if (drawContext == null) throw new ArgumentNullException(nameof(drawContext));
            if (texture == null) throw new ArgumentNullException(nameof(texture));
            if (drawParameters == null) throw new ArgumentNullException(nameof(drawParameters));
            
            drawContext.Draw(
                texture,
                drawParameters.Position, 
                drawParameters.DestinationRectangle,
                drawParameters.SourceRectangle,
                drawParameters.Origin,
                drawParameters.Rotation,
                drawParameters.Scale,
                drawParameters.Color,
                drawParameters.Effects,
                drawParameters.LayerDepth);
        }
    }
}