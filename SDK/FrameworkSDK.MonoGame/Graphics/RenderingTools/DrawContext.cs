

using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.RenderingTools
{
    public class DrawContext : IDrawContext
    {
	    private SpriteBatch SpriteBatch { get; }

	    public DrawContext(SpriteBatch spriteBatch)
	    {
		    SpriteBatch = spriteBatch;
	    }
	    
	    public void Dispose()
	    {
		    
	    }

	    public void Draw(Texture2D texture, Vector2? position = null, Rectangle? destinationRectangle = null,
		    Rectangle? sourceRectangle = null, Vector2? origin = null, float rotation = 0, Vector2? scale = null,
		    Color? color = null, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0)
	    {
		    SpriteBatch.Draw(texture, position, destinationRectangle, sourceRectangle, origin, rotation, scale, color, effects, layerDepth);
	    }

	    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin,
		    Vector2 scale, SpriteEffects effects, float layerDepth)
	    {
		    SpriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
	    }

	    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin,
		    float scale, SpriteEffects effects, float layerDepth)
	    {
		    SpriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
	    }

	    public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation,
		    Vector2 origin, SpriteEffects effects, float layerDepth)
	    {
		    SpriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
	    }

	    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
	    {
		    SpriteBatch.Draw(texture, position, sourceRectangle, color);
	    }

	    public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
	    {
		    SpriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
	    }

	    public void Draw(Texture2D texture, Vector2 position, Color color)
	    {
		    SpriteBatch.Draw(texture, position, color);
	    }

	    public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
	    {
		    SpriteBatch.Draw(texture, destinationRectangle, color);
	    }

	    public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
	    {
		    SpriteBatch.DrawString(spriteFont, text, position, color);
	    }

	    public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin,
		    float scale, SpriteEffects effects, float layerDepth)
	    {
		    SpriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
	    }

	    public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin,
		    Vector2 scale, SpriteEffects effects, float layerDepth)
	    {
		    SpriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
	    }

	    public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color)
	    {
		    SpriteBatch.DrawString(spriteFont, text, position, color);
	    }

	    public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation,
		    Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
	    {
		    SpriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
	    }

	    public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation,
		    Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
	    {
		    SpriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
	    }
    }
}
