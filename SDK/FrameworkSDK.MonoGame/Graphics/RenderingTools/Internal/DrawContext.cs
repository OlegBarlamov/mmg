using System;
using System.Text;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.RenderingTools
{
    internal class DrawContext : IDrawContext
    {
	    private SpriteBatch SpriteBatch { get; }
	    private ICamera2DProvider Camera2DProvider { get; }

	    [CanBeNull] public ICamera2D CameraOverride { get; set; } = null;

	    public DrawContext(SpriteBatch spriteBatch, [NotNull] ICamera2DProvider camera2DProvider)
	    {
		    SpriteBatch = spriteBatch;
		    Camera2DProvider = camera2DProvider ?? throw new ArgumentNullException(nameof(camera2DProvider));
	    }
	    
	    public void Dispose()
	    {
		    
	    }
	    
	    private ICamera2D GetActiveCamera()
	    {
		    return CameraOverride ?? Camera2DProvider.GetScreenCamera();
	    }

	    public void Draw(Texture2D texture, Vector2? position = null, Rectangle? destinationRectangle = null,
		    Rectangle? sourceRectangle = null, Vector2? origin = null, float rotation = 0, Vector2? scale = null,
		    Color? color = null, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0)
	    {
		    SpriteBatch.Draw(texture, GetActiveCamera().ToDisplay(position), GetActiveCamera().ToDisplay(destinationRectangle), sourceRectangle, origin, rotation, scale, color, effects, layerDepth);
	    }

	    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin,
		    Vector2 scale, SpriteEffects effects, float layerDepth)
	    {
		    SpriteBatch.Draw(texture, GetActiveCamera().ToDisplay(position), sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
	    }

	    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin,
		    float scale, SpriteEffects effects, float layerDepth)
	    {
		    SpriteBatch.Draw(texture, GetActiveCamera().ToDisplay(position), sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
	    }

	    public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation,
		    Vector2 origin, SpriteEffects effects, float layerDepth)
	    {
		    SpriteBatch.Draw(texture, GetActiveCamera().ToDisplay(destinationRectangle), sourceRectangle, color, rotation, origin, effects, layerDepth);
	    }

	    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
	    {
		    SpriteBatch.Draw(texture, GetActiveCamera().ToDisplay(position), sourceRectangle, color);
	    }

	    public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
	    {
		    SpriteBatch.Draw(texture, GetActiveCamera().ToDisplay(destinationRectangle), sourceRectangle, color);
	    }

	    public void Draw(Texture2D texture, Vector2 position, Color color)
	    {
		    SpriteBatch.Draw(texture, GetActiveCamera().ToDisplay(position), color);
	    }

	    public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
	    {
		    SpriteBatch.Draw(texture, GetActiveCamera().ToDisplay(destinationRectangle), color);
	    }

	    public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
	    {
		    SpriteBatch.DrawString(spriteFont, text, GetActiveCamera().ToDisplay(position), color);
	    }

	    public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin,
		    float scale, SpriteEffects effects, float layerDepth)
	    {
		    SpriteBatch.DrawString(spriteFont, text, GetActiveCamera().ToDisplay(position), color, rotation, origin, scale, effects, layerDepth);
	    }

	    public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin,
		    Vector2 scale, SpriteEffects effects, float layerDepth)
	    {
		    SpriteBatch.DrawString(spriteFont, text, GetActiveCamera().ToDisplay(position), color, rotation, origin, scale, effects, layerDepth);
	    }

	    public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color)
	    {
		    SpriteBatch.DrawString(spriteFont, text, GetActiveCamera().ToDisplay(position), color);
	    }

	    public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation,
		    Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
	    {
		    SpriteBatch.DrawString(spriteFont, text, GetActiveCamera().ToDisplay(position), color, rotation, origin, scale, effects, layerDepth);
	    }

	    public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation,
		    Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
	    {
		    SpriteBatch.DrawString(spriteFont, text, GetActiveCamera().ToDisplay(position), color, rotation, origin, scale, effects, layerDepth);
	    }
    }
}
