using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atom.Client.MacOS
{
  public class Game1 : Game
  {
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    public Game1 ()
    {
      graphics = new GraphicsDeviceManager (this);
      Content.RootDirectory = "Content";
      graphics.IsFullScreen = false;
    }


    protected override void Initialize()
    {
      base.Initialize();
      
      graphics.PreferredBackBufferWidth = 800;
      graphics.PreferredBackBufferHeight = 600;
      graphics.ApplyChanges();
    }

    protected override void LoadContent ()
    {
      spriteBatch = new SpriteBatch (GraphicsDevice);
    }

    protected override void Update (GameTime gameTime)
    {
      base.Update (gameTime);
    }

    protected override void Draw (GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      base.Draw (gameTime);
    }
  }
}