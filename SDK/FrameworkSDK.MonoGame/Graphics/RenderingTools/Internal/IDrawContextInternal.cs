using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics
{
    public interface IDrawContextInternal
    {
	    void Draw(Texture2D texture, Vector2? position = null, Rectangle? destinationRectangle = null,
		    Rectangle? sourceRectangle = null, Vector2? origin = null, float rotation = 0.0f, Vector2? scale = null,
		    Color? color = null, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0.0f);
	    
	    void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation,
		    Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
	    
	    void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation,
		    Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
	    
	    void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color,
		    float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
	    
	    void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color);
	    
	    void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color);
	    
	    void Draw(Texture2D texture, Vector2 position, Color color);
	    
	    void Draw(Texture2D texture, Rectangle destinationRectangle, Color color);
	    
	    void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color);
	    
	    void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin,
		    float scale, SpriteEffects effects, float layerDepth);
	    
	    void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin,
		    Vector2 scale, SpriteEffects effects, float layerDepth);
	    
	    void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color);
	    
	    void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation,
		    Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
	    
	    void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation,
		    Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);

    }
}
