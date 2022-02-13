using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Console.Core;
using Console.Core.Models;
using Console.InGame;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameExtensions;

namespace Atom.Client.MacOS
{
  internal class FakeMessagesProvider : IConsoleMessagesProvider
  {
    public class ConsoleMessage : IConsoleMessage
    {
      public string Message { get; }

      public ConsoleMessage(string message, ConsoleLogLevel logLevel, object content)
      {
        Message = message;
        LogLevel = logLevel;
        Content = content;
      }
      
      public string Source { get; } = "Fake";
      public ConsoleLogLevel LogLevel { get; }
      public object Content { get; } = null;
    }
    
    public event Action NewMessages;
    public bool IsQueueEmpty => _messages.IsEmpty;
    
    private ConcurrentQueue<IConsoleMessage> _messages = new ConcurrentQueue<IConsoleMessage>();
    private Random _random = new Random();

    private readonly Timer _timer;
    
    public FakeMessagesProvider()
    {
      _timer = new Timer(TimerOnElapsed, new object(), 500, 500);
    }

    public void SendMessage(ConsoleMessage message)
    {
      _messages.Enqueue(message);
    }

    private static int _counter;
    
    private void TimerOnElapsed(object state)
    {
      try
      {
        var text = (_counter++).ToString();//_random.NextDouble().ToString(CultureInfo.InvariantCulture);
        //_messages.Enqueue(new ConsoleMessage(text, ConsoleLogLevel.Information, null));
        NewMessages?.Invoke();
      }
      catch (Exception exception)
      {
        System.Console.WriteLine(exception);
        throw;
      }
    }

    public IConsoleMessage Pop()
    {
      _messages.TryDequeue(out var item);
      return item;
    }
  }

  internal class FakeCommandExecutor : IConsoleCommandExecutor
  {
    private class ConsoleCommand : IConsoleCommand
    {
      private class MetadataClass : IConsoleCommandMetadata
      {
        public string Title { get; }
        public string Description { get; }
        public object Data { get; }

        public MetadataClass(string title, string description, object data = null)
        {
          Title = title;
          Description = description;
          Data = data;
        }
      }
      
      public string Text { get; }
      public IConsoleCommandMetadata Metadata { get; }

      public ConsoleCommand(string text, string signature, string description, object data = null)
      {
        Text = text;
        Metadata = new MetadataClass(signature, description, data);
      }
    }

    public class MessageData
    {
      public Texture2D Texture { get; }
      public Vector2 Size { get; }
      public Color Color { get; }

      public MessageData(Texture2D texture, Vector2 size, Color color)
      {
        Texture = texture;
        Size = size;
        Color = color;
      }
    }
    
    public Action<FakeMessagesProvider.ConsoleMessage> AddMessage { get; }
    public Texture2D OnePixelTexture { get; }

    public FakeCommandExecutor([NotNull] Action<FakeMessagesProvider.ConsoleMessage> addMessage,
      [NotNull] Texture2D onePixelRenderTarget)
    {
      AddMessage = addMessage ?? throw new ArgumentNullException(nameof(addMessage));
      OnePixelTexture = onePixelRenderTarget ?? throw new ArgumentNullException(nameof(onePixelRenderTarget));
    }
    
    public IEnumerable<IConsoleCommand> GetAvailableCommands()
    {
      return new[]
      {
        new ConsoleCommand("write", "write [message]", "Write message to console output"),
        new ConsoleCommand("without_all", null, null),
        new ConsoleCommand("without_desc", "without_desc", null),
        new ConsoleCommand("do", "do [action]", "Do something with you."),
        new ConsoleCommand("minus", "minus [number]", "Minus"),
        new ConsoleCommand("plus", "plus [number]", "Plus"),
        new ConsoleCommand("show", "show", "Show console window"),
        new ConsoleCommand("hide", "hide", "Hide console window"),
        new ConsoleCommand("aaaa", "aaaa aaaa", "aaaa aaaa aaaa aaaa aaaa aaaa aaaa aaaa aaaa aaaa"),
        new ConsoleCommand("aaa", "aaa aaa", "aaa aaa aaa aaa aaa aaa aaa aaa aaa aaa"),
        new ConsoleCommand("aaaaa", "aaaaa aaaaa", "aaaaa aaaaa aaaaa aaaaa aaaaa aaaaa aaaaa aaaaa aaaaa aaaaa"),
        new ConsoleCommand("aaaaaaa", "aaaaaaa aaaaaaa", "aaaaaaa aaaaaaa aaaaaaa aaaaaaa aaaaaaa aaaaaaa aaaaaaa aaaaaaa aaaaaaa aaaaaaa"),
        new ConsoleCommand("aaaaabbaa", "aaaaabbaa aaaaabbaa", "aaaaabbaa aaaaabbaa aaaaabbaa aaaaabbaa aaaaabbaa aaaaabbaa aaaaabbaa aaaaabbaa aaaaabbaa aaaaabbaa"),
        new ConsoleCommand("aabbaaabbaa", "aabbaaabbaa aabbaaabbaa", "aabbaaabbaa aabbaaabbaa aabbaaabbaa aabbaaabbaa aabbaaabbaa aabbaaabbaa aabbaaabbaa aabbaaabbaa aabbaaabbaa aabbaaabbaa"),
        new ConsoleCommand("aabbavaabbaa", "aabbavaabbaa aabbavaabbaa", "aabbavaabbaa aabbavaabbaa aabbavaabbaa aabbavaabbaa aabbavaabbaa aabbavaabbaa aabbavaabbaa aabbavaabbaa aabbavaabbaa aabbavaabbaa"),
        new ConsoleCommand("babbavaabbaa", "babbavaabbaa babbavaabbaa", "babbavaabbaa babbavaabbaa babbavaabbaa babbavaabbaa babbavaabbaa babbavaabbaa babbavaabbaa babbavaabbaa babbavaabbaa babbavaabbaa"),
        new ConsoleCommand("bbbbavaabbaa", "bbbbavaabbaa bbbbavaabbaa", "bbbbavaabbaa bbbbavaabbaa bbbbavaabbaa bbbbavaabbaa bbbbavaabbaa bbbbavaabbaa bbbbavaabbaa bbbbavaabbaa bbbbavaabbaa bbbbavaabbaa"),
        new ConsoleCommand("draw", "draw [red] [green] [blue] [width] [height]", "draw solid texture to console", new MessageData(OnePixelTexture, new Vector2(1000,1000), Color.Purple)), 
      };
    }

    public Task ExecuteAsync(string command)
    {
      if (!command.StartsWith("draw "))
      {
        AddMessage.Invoke(new FakeMessagesProvider.ConsoleMessage("Unknown command: '" + command + "'", ConsoleLogLevel.Information, command));
        return Task.CompletedTask;
      }

      return Task.Factory.StartNew(() =>
      {
        try
        {
          var words = command.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
          var red = int.Parse(words[1]);
          var green = int.Parse(words[2]);
          var blue = int.Parse(words[3]);
          var width = int.Parse(words[4]);
          var height = int.Parse(words[5]);
          
          var data = new MessageData(OnePixelTexture, new Vector2(width, height), Color.FromNonPremultiplied(red, green, blue, 255));
          AddMessage.Invoke(new FakeMessagesProvider.ConsoleMessage("Image:", ConsoleLogLevel.Warning, data));
        }
        catch (Exception e)
        {
          var message = new FakeMessagesProvider.ConsoleMessage(e.ToString(), ConsoleLogLevel.Error, null);
          AddMessage(message);
        }
      });
    }
  }

  internal class DataRenderer : CustomDataRenderer<FakeCommandExecutor.MessageData>, IDataRenderer
  {
    protected override Vector2 Measure(FakeCommandExecutor.MessageData data, Vector2 availableSize)
    {
      return data.Size;
    }

    protected override void Draw(FakeCommandExecutor.MessageData data, GameTime gameTime, SpriteBatch spriteBatch, Rectangle rectangle)
    {
      spriteBatch.Draw(data.Texture, rectangle, data.Color);
    }
  }
  
  public class Game1 : Game
  {
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    private InGameConsoleConfig _consoleConfig;
    private InGameConsoleController _consoleController;

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
      var background = new Color(31,32,36);
      var headerAmbient = new Color(26, 54, 84);
      _consoleConfig = new InGameConsoleConfig
      {
        DefaultWidth = Window.ClientBounds.Width,
        HeaderBackground = GraphicsDevice.GetTextureGradientColor(headerAmbient, background, 30, 20, 90, 0.8f),
        Background = GraphicsDevice.GetTextureDiffuseColor(background),
        CommandLineCorner = GraphicsDevice.GetTextureDiffuseColor(Color.White),
        SuggestSelection = GraphicsDevice.GetTextureDiffuseColor(Color.Orange),
        ConsoleFont = Content.Load<SpriteFont>("ConsoleFont")
      };

      var onePixelTexture = GraphicsDevice.GetTextureDiffuseColor(Color.White);
      var messagesProvider = new FakeMessagesProvider();
      _consoleController = new InGameConsoleController(messagesProvider,  new FakeCommandExecutor(messagesProvider.SendMessage, onePixelTexture));
      _consoleController.RegisterDataRenderer(new DataRenderer());
      _consoleController.Initialize(_consoleConfig, GraphicsDevice);
    }

    private bool _isDown;
    
    protected override void Update (GameTime gameTime)
    {
      base.Update (gameTime);

      if (Keyboard.GetState().IsKeyDown(Keys.OemTilde))
      {
        if (!_isDown)
        {
          if (_consoleController.IsShowed)
            _consoleController.Hide();
          else
            _consoleController.Show();
        }

        _isDown = true;
      }
      else _isDown = false;

      _consoleController.Update(gameTime);
    }

    protected override void Draw (GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);
      _consoleController.Draw(gameTime, spriteBatch);

      base.Draw (gameTime);
    }
  }
}