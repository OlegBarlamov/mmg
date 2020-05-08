using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics
{
    public interface IDrawContext : IDisposable
    {
	    /// <summary>Submit a sprite for drawing in the current batch.</summary>
	    /// <param name="texture">A texture.</param>
	    /// <param name="position">The drawing location on screen or null if <paramref name="destinationRectangle"> is used.</paramref></param>
	    /// <param name="destinationRectangle">The drawing bounds on screen or null if <paramref name="position"> is used.</paramref></param>
	    /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
	    /// <param name="origin">An optional center of rotation. Uses <see cref="P:Microsoft.Xna.Framework.Vector2.Zero" /> if null.</param>
	    /// <param name="rotation">An optional rotation of this sprite. 0 by default.</param>
	    /// <param name="scale">An optional scale vector. Uses <see cref="P:Microsoft.Xna.Framework.Vector2.One" /> if null.</param>
	    /// <param name="color">An optional color mask. Uses <see cref="P:Microsoft.Xna.Framework.Color.White" /> if null.</param>
	    /// <param name="effects">The optional drawing modificators. <see cref="F:Microsoft.Xna.Framework.Graphics.SpriteEffects.None" /> by default.</param>
	    /// <param name="layerDepth">An optional depth of the layer of this sprite. 0 by default.</param>
	    /// <exception cref="T:System.InvalidOperationException">Throwns if both <paramref name="position" /> and <paramref name="destinationRectangle" /> been used.</exception>
	    /// <remarks>This overload uses optional parameters. This overload requires only one of <paramref name="position" /> and <paramref name="destinationRectangle" /> been used.</remarks>
	    void Draw(Texture2D texture, Vector2? position = null, Rectangle? destinationRectangle = null,
		    Rectangle? sourceRectangle = null, Vector2? origin = null, float rotation = 0.0f, Vector2? scale = null,
		    Color? color = null, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0.0f);

	    /// <summary>Submit a sprite for drawing in the current batch.</summary>
	    /// <param name="texture">A texture.</param>
	    /// <param name="position">The drawing location on screen.</param>
	    /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
	    /// <param name="color">A color mask.</param>
	    /// <param name="rotation">A rotation of this sprite.</param>
	    /// <param name="origin">Center of the rotation. 0,0 by default.</param>
	    /// <param name="scale">A scaling of this sprite.</param>
	    /// <param name="effects">Modificators for drawing. Can be combined.</param>
	    /// <param name="layerDepth">A depth of the layer of this sprite.</param>
	    void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation,
		    Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);


	    /// <summary>Submit a sprite for drawing in the current batch.</summary>
	    /// <param name="texture">A texture.</param>
	    /// <param name="position">The drawing location on screen.</param>
	    /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
	    /// <param name="color">A color mask.</param>
	    /// <param name="rotation">A rotation of this sprite.</param>
	    /// <param name="origin">Center of the rotation. 0,0 by default.</param>
	    /// <param name="scale">A scaling of this sprite.</param>
	    /// <param name="effects">Modificators for drawing. Can be combined.</param>
	    /// <param name="layerDepth">A depth of the layer of this sprite.</param>
	    void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation,
		    Vector2 origin, float scale, SpriteEffects effects, float layerDepth);

	    /// <summary>Submit a sprite for drawing in the current batch.</summary>
	    /// <param name="texture">A texture.</param>
	    /// <param name="destinationRectangle">The drawing bounds on screen.</param>
	    /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
	    /// <param name="color">A color mask.</param>
	    /// <param name="rotation">A rotation of this sprite.</param>
	    /// <param name="origin">Center of the rotation. 0,0 by default.</param>
	    /// <param name="effects">Modificators for drawing. Can be combined.</param>
	    /// <param name="layerDepth">A depth of the layer of this sprite.</param>
	    void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color,
		    float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);

	    /// <summary>Submit a sprite for drawing in the current batch.</summary>
	    /// <param name="texture">A texture.</param>
	    /// <param name="position">The drawing location on screen.</param>
	    /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
	    /// <param name="color">A color mask.</param>
	    void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color);

	    /// <summary>Submit a sprite for drawing in the current batch.</summary>
	    /// <param name="texture">A texture.</param>
	    /// <param name="destinationRectangle">The drawing bounds on screen.</param>
	    /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
	    /// <param name="color">A color mask.</param>
	    void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color);

	    /// <summary>Submit a sprite for drawing in the current batch.</summary>
	    /// <param name="texture">A texture.</param>
	    /// <param name="position">The drawing location on screen.</param>
	    /// <param name="color">A color mask.</param>
	    void Draw(Texture2D texture, Vector2 position, Color color);

	    /// <summary>Submit a sprite for drawing in the current batch.</summary>
	    /// <param name="texture">A texture.</param>
	    /// <param name="destinationRectangle">The drawing bounds on screen.</param>
	    /// <param name="color">A color mask.</param>
	    void Draw(Texture2D texture, Rectangle destinationRectangle, Color color);

	    /// <summary>
	    /// Submit a text string of sprites for drawing in the current batch.
	    /// </summary>
	    /// <param name="spriteFont">A font.</param>
	    /// <param name="text">The text which will be drawn.</param>
	    /// <param name="position">The drawing location on screen.</param>
	    /// <param name="color">A color mask.</param>
	    void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color);

	    /// <summary>
	    /// Submit a text string of sprites for drawing in the current batch.
	    /// </summary>
	    /// <param name="spriteFont">A font.</param>
	    /// <param name="text">The text which will be drawn.</param>
	    /// <param name="position">The drawing location on screen.</param>
	    /// <param name="color">A color mask.</param>
	    /// <param name="rotation">A rotation of this string.</param>
	    /// <param name="origin">Center of the rotation. 0,0 by default.</param>
	    /// <param name="scale">A scaling of this string.</param>
	    /// <param name="effects">Modificators for drawing. Can be combined.</param>
	    /// <param name="layerDepth">A depth of the layer of this string.</param>
	    void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin,
		    float scale, SpriteEffects effects, float layerDepth);


	    /// <summary>
	    /// Submit a text string of sprites for drawing in the current batch.
	    /// </summary>
	    /// <param name="spriteFont">A font.</param>
	    /// <param name="text">The text which will be drawn.</param>
	    /// <param name="position">The drawing location on screen.</param>
	    /// <param name="color">A color mask.</param>
	    /// <param name="rotation">A rotation of this string.</param>
	    /// <param name="origin">Center of the rotation. 0,0 by default.</param>
	    /// <param name="scale">A scaling of this string.</param>
	    /// <param name="effects">Modificators for drawing. Can be combined.</param>
	    /// <param name="layerDepth">A depth of the layer of this string.</param>
	    void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin,
		    Vector2 scale, SpriteEffects effects, float layerDepth);

	    /// <summary>
	    /// Submit a text string of sprites for drawing in the current batch.
	    /// </summary>
	    /// <param name="spriteFont">A font.</param>
	    /// <param name="text">The text which will be drawn.</param>
	    /// <param name="position">The drawing location on screen.</param>
	    /// <param name="color">A color mask.</param>
	    void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color);

	    /// <summary>
	    /// Submit a text string of sprites for drawing in the current batch.
	    /// </summary>
	    /// <param name="spriteFont">A font.</param>
	    /// <param name="text">The text which will be drawn.</param>
	    /// <param name="position">The drawing location on screen.</param>
	    /// <param name="color">A color mask.</param>
	    /// <param name="rotation">A rotation of this string.</param>
	    /// <param name="origin">Center of the rotation. 0,0 by default.</param>
	    /// <param name="scale">A scaling of this string.</param>
	    /// <param name="effects">Modificators for drawing. Can be combined.</param>
	    /// <param name="layerDepth">A depth of the layer of this string.</param>
	    void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation,
		    Vector2 origin, float scale, SpriteEffects effects, float layerDepth);


	    /// <summary>
	    /// Submit a text string of sprites for drawing in the current batch.
	    /// </summary>
	    /// <param name="spriteFont">A font.</param>
	    /// <param name="text">The text which will be drawn.</param>
	    /// <param name="position">The drawing location on screen.</param>
	    /// <param name="color">A color mask.</param>
	    /// <param name="rotation">A rotation of this string.</param>
	    /// <param name="origin">Center of the rotation. 0,0 by default.</param>
	    /// <param name="scale">A scaling of this string.</param>
	    /// <param name="effects">Modificators for drawing. Can be combined.</param>
	    /// <param name="layerDepth">A depth of the layer of this string.</param>
	    void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation,
		    Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);

    }
}
