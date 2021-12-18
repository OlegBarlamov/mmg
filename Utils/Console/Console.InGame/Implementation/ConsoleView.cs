using System;
using System.Collections.Generic;
using System.Linq;
using Console.Core.Models;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Console.InGame.Implementation
{
    internal class ConsoleView : IDisposable
    {
        public event Action ShowAnimationFinished;
        public event Action HideAnimationFinished;

        private ConsoleModel Model { get; }
        private InGameConsoleConfig Config { get; set; }
        private GraphicsDevice GraphicsDevice { get; set; }
        private RenderersProvider RenderersProvider { get; }

        private string _lastEnteringText;
        private Vector2 _enteringTextSize;
        private int _lastCaretIndex = -1;
        private Vector2 _caretPosition;
        private bool _isShowAnimationActive;
        private bool _isHideAnimationActive;
        private float _actualHeight;
        private float _actualWidth;
        
        private string _caretSymbol = "_";
        private bool _caretVisible;
        private TimeSpan _caretVisibleTimeSpan = TimeSpan.Zero;

        private bool _isFirstDraw = true;

        private IConsoleCommand _lastVisibleHelpCommand;
        private Vector2 _commandHelpSize;
        
        private Vector2 _commandSuggestionsSize;

        private RenderTarget2D _renderTargetForConsoleMessages;
        private RenderTarget2D _renderTargetForCommandHelp;

        private bool _initialized;
        
        public ConsoleView([NotNull] ConsoleModel model, [NotNull] RenderersProvider renderersProvider)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            RenderersProvider = renderersProvider ?? throw new ArgumentNullException(nameof(renderersProvider));
        }

        public void Initialize([NotNull] InGameConsoleConfig config,[NotNull] GraphicsDevice graphicsDevice)
        {
            if (_initialized) throw new InGameConsoleException("Console initialized already");
            _initialized = true;
            
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            
            _renderTargetForConsoleMessages = CreateRenderTarget();
            _renderTargetForCommandHelp = CreateRenderTarget();
        }

        public void Dispose()
        {
            ShowAnimationFinished = null;
            _renderTargetForConsoleMessages.Dispose();
            _renderTargetForCommandHelp.Dispose();
        }

        public void Show()
        {
            if (!_initialized) throw new InGameConsoleException("Console not initialized yet");
            _actualWidth = Model.Width;
            _isHideAnimationActive = false;
            _isShowAnimationActive = true;
        }

        public void Hide()
        {
            if (!_initialized) throw new InGameConsoleException("Console not initialized yet");
            _isShowAnimationActive = false;
            _isHideAnimationActive = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!_initialized) throw new InGameConsoleException("Console not initialized yet");
            LazyDraw(gameTime, spriteBatch);
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            
            DrawBackground(gameTime, spriteBatch);
            DrawPreparedConsoleMessages(gameTime, spriteBatch);
            DrawCommandLine(gameTime, spriteBatch);
            DrawCommandSuggestions(gameTime, spriteBatch);
            DrawPreparedCommandHelp(gameTime, spriteBatch);
            
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            if (!_initialized) throw new InGameConsoleException("Console not initialized yet");
            ProcessAnimation(gameTime);

            _caretVisibleTimeSpan += gameTime.ElapsedGameTime;
            if (_caretVisibleTimeSpan.TotalMilliseconds > 500)
            {
                _caretVisible = !_caretVisible;
                _caretVisibleTimeSpan = TimeSpan.Zero;
            }

            if (Model.SuggestionIndexChanged)
                CalcSuggestionsSize();
        }

        private void LazyDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Model.IsVisibleMessagesChanged || _isFirstDraw)
            {
                DrawConsoleMessagesToRenderTarget(gameTime, spriteBatch);
            }

            if (Model.HelpCommand != _lastVisibleHelpCommand)
            {
                _lastVisibleHelpCommand = Model.HelpCommand;
                DrawCommandHelpToRenderTarget(gameTime, spriteBatch);
            }

            _isFirstDraw = false;
        }

        private void ProcessAnimation(GameTime gameTime)
        {
            if (_isShowAnimationActive)
            {
                var elapsed = gameTime.ElapsedGameTime.TotalMilliseconds;
                var fullHeight = Model.Height;
                var animationTime = Config.AnimationTime.TotalMilliseconds;
                var heightIncreaseByMillisecond = fullHeight / animationTime;
                var increaseNow = heightIncreaseByMillisecond * elapsed;
                _actualHeight += _actualHeight = (float)increaseNow;
                if (_actualHeight >= Model.Height)
                {
                    _actualHeight = Model.Height;
                    _isShowAnimationActive = false;
                    ShowAnimationFinished?.Invoke();
                }
            }

            if (_isHideAnimationActive)
            {
                var elapsed = gameTime.ElapsedGameTime.TotalMilliseconds;
                var fullHeight = Model.Height;
                var animationTime = Config.AnimationTime.TotalMilliseconds;
                var heightDecreaseByMillisecond = fullHeight / animationTime;
                var decreaseNow = heightDecreaseByMillisecond * elapsed;
                _actualHeight -= _actualHeight = (float)decreaseNow;
                if (_actualHeight <= 0)
                {
                    _actualHeight = 0;
                    _actualWidth = 0;
                    _isHideAnimationActive = false;
                    HideAnimationFinished?.Invoke();
                }
            }
        }
        
        private void CalcSuggestionsSize()
        {
            var size = new Vector2(0, Config.ConsoleMessagesInterval);

            foreach (var commandSuggestion in Model.VisibleCommandSuggestions)
            {
                var text = commandSuggestion.Command.Text;
                var commandSize = Config.ConsoleFont.MeasureString(text);
                size.Y += commandSize.Y += Config.ConsoleMessagesInterval;
                size.X = Math.Max(size.X, commandSize.X);
            }

            _commandSuggestionsSize = new Vector2(size.X + Config.ConsoleTextPadding * 2, size.Y);
        }

        private void DrawConsoleMessagesToRenderTarget(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GraphicsDevice.SetRenderTarget(_renderTargetForConsoleMessages);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            DrawConsoleMessages(gameTime, spriteBatch, Model.VisibleMessages);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        private void DrawPreparedConsoleMessages(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var textPosition = new Vector2(
                Model.Position.Y + Config.ConsoleTextPadding,
                Model.Position.X + Config.ConsoleTextPadding);
            var rect = new Rectangle(
                (int)Model.VisibleTextOffset.X,
                (int)Model.VisibleTextOffset.Y,
                (int) (_actualWidth - Config.ConsoleTextPadding * 2),
                (int) (_actualHeight - Config.ConsoleTextPadding * 2 - Config.ConsoleCommandLineSectionHeight));
            
            spriteBatch.Draw(_renderTargetForConsoleMessages, textPosition, rect, Color.White  * Config.Opacity);
        }
        
        private void DrawBackground(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var minHeight = Math.Min(_actualHeight, Config.MinHeight);
            var headerSize = new Vector2(_actualWidth, minHeight);
            spriteBatch.Draw(Config.HeaderBackground, new Rectangle(Model.Position.ToPoint(), headerSize.ToPoint()), Color.White * Config.Opacity);

            var pos = Model.Position + new Vector2(0, minHeight);
            var size = new Vector2(_actualWidth,_actualHeight - minHeight);
            spriteBatch.Draw(Config.Background, new Rectangle(pos.ToPoint(), size.ToPoint()), Color.White * Config.Opacity);
        }

        private void DrawConsoleMessages(GameTime gameTime, SpriteBatch spriteBatch,
            IEnumerable<IRenderingMessage> consoleMessages)
        {
            var x = 0f;
            var y = 0f;
            foreach (var message in consoleMessages)
            {
                var pos = new Vector2(x, y);
                var availableSize = new Vector2(_renderTargetForConsoleMessages.Width, _renderTargetForConsoleMessages.Height);
                message.Draw(gameTime, spriteBatch, pos, availableSize);
                y += message.Size.Y + Config.ConsoleMessagesInterval;
            }
        }

        private void DrawCommandLine(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var margin = new Rectangle(10, 10, 10, 10);
            var height = Config.ConsoleCommandLineSectionHeight - margin.Left - margin.Height;
            
            var left = Model.Position.X + margin.Left;
            var top = Model.Position.Y + _actualHeight - height - margin.Height;
            var width = _actualWidth - margin.Right;
            var commandLineCorner = new Rectangle((int)left,(int)top, (int)width, (int)height);
            
            DrawRectangleCorner(gameTime, spriteBatch, Config.CommandLineCorner, commandLineCorner, (int)Config.CommandLineBoarderSize);
            
            DrawCommandLineCommand(gameTime, spriteBatch, commandLineCorner);
        }

        private void DrawRectangleCorner(GameTime gameTime, SpriteBatch spriteBatch, Texture2D texture,
            Rectangle rectangle, int cornerSize)
        {
            spriteBatch.Draw(texture, new Rectangle(rectangle.Left, rectangle.Top, cornerSize, rectangle.Height), Color.White* Config.Opacity); // Left
            spriteBatch.Draw(texture, new Rectangle(rectangle.Right, rectangle.Top, cornerSize, rectangle.Height), Color.White* Config.Opacity); // Right
            spriteBatch.Draw(texture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width , cornerSize), Color.White* Config.Opacity); // Top
            spriteBatch.Draw(texture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, cornerSize), Color.White* Config.Opacity); // Bottom    
        }

        private void DrawCommandLineCommand(GameTime gameTime, SpriteBatch spriteBatch, Rectangle commandLineRect)
        {
            var text = Model.EnteringCommandText ?? string.Empty;
            if (text != _lastEnteringText || _lastEnteringText == null)
            {
                _enteringTextSize = Config.ConsoleFont.MeasureString(text);
                _lastEnteringText = text;

                if (Model.VisibleCommandSuggestions.Count > 0)
                {
                    CalcSuggestionsSize();
                }
            }

            var xOffset = commandLineRect.Left + Config.ConsoleTextPadding / 2;
            var yOffset = commandLineRect.Top + commandLineRect.Height / 2;
            var position = new Vector2(xOffset, yOffset - _enteringTextSize.Y / 2 + 3f);
            spriteBatch.DrawString(
                Config.ConsoleFont,
                text,
                position,
                Color.White* Config.Opacity);
            
            var caretY = commandLineRect.Top + commandLineRect.Height - 15f;
            var caretX = _caretPosition.X;
            if (_lastCaretIndex != Model.CaretPosition)
            {
                _lastCaretIndex = Model.CaretPosition;
                if (_lastCaretIndex > text.Length || _lastCaretIndex <= 0)
                {
                    caretX = xOffset;
                }
                else
                {
                    var subString = text.Substring(0, _lastCaretIndex);
                    var subStringSize = Config.ConsoleFont.MeasureString(subString);
                    caretX = xOffset + subStringSize.X;
                }
            }

            _caretPosition = new Vector2(caretX, caretY);
            if (_caretVisible)
            {
                spriteBatch.DrawString(
                    Config.ConsoleFont,
                    _caretSymbol,
                    _caretPosition,
                    Color.White* Config.Opacity);
            }
        }

        private void DrawCommandSuggestions(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Model.VisibleCommandSuggestions.Count < 1)
                return;

            var rec = new Rectangle(
                (int)Config.Position.X,
                (int)(Config.Position.Y + _actualHeight),
                (int)_commandSuggestionsSize.X,
                (int)_commandSuggestionsSize.Y);
            spriteBatch.Draw(Config.Background, rec, Color.White * Config.Opacity);

            var heightBySuggestion = (_commandSuggestionsSize.Y - Config.ConsoleMessagesInterval) / Model.VisibleCommandSuggestions.Count;
            var pos = new Vector2(Config.Position.X + Config.ConsoleTextPadding,  Config.Position.Y + _actualHeight + Config.ConsoleMessagesInterval);
            var index = 0;
            foreach (var commandSuggestion in Model.VisibleCommandSuggestions)
            {
                if (index == Model.VisibleSelectedSuggestionIndex)
                {
                    var selectionRect = new Rectangle(
                        rec.Left,
                        (int)(pos.Y - Config.ConsoleMessagesInterval),
                        rec.Width,
                        (int)heightBySuggestion);
                    spriteBatch.Draw(Config.SuggestSelection, selectionRect, Color.White * Config.Opacity);
                }
                spriteBatch.DrawString(Config.ConsoleFont, commandSuggestion.Command.Text, pos, Color.White * Config.Opacity);
                pos.Y += heightBySuggestion;
                index++;
            }
        }

        private void DrawCommandHelpToRenderTarget(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Model.HelpCommand == null)
                return;
            
            GraphicsDevice.SetRenderTarget(_renderTargetForCommandHelp);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            DrawCommandHelp(gameTime, spriteBatch);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }
        
        private void DrawPreparedCommandHelp(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Model.HelpCommand == null)
                return;

            var height = Math.Max(_commandHelpSize.Y, _commandSuggestionsSize.Y);
            var rec = new Rectangle(
                0,
                0,
                (int)(_actualWidth - _commandSuggestionsSize.X - Config.ConsoleMessagesInterval),
                (int)height);

            var left = Config.Position.X + _commandSuggestionsSize.X + Config.ConsoleMessagesInterval;
            var top = Config.Position.Y + _actualHeight;
            spriteBatch.Draw(Config.Background, new Rectangle((int)left, (int)top, rec.Width, rec.Height), Color.White * Config.Opacity);
            
            spriteBatch.Draw(
                _renderTargetForCommandHelp, 
                new Vector2(left,top),
                rec, Color.White * Config.Opacity);
        }

        private void DrawCommandHelp(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Model.HelpCommand?.Metadata == null)
                return;
            
            var left = 0;
            var top = 0;
            var width = _actualWidth - _commandSuggestionsSize.X - Config.ConsoleMessagesInterval;

            var title = Model.HelpCommand.Metadata.Title;
            if (string.IsNullOrWhiteSpace(title))
                title = Model.HelpCommand.Text;
            
            var description = Model.HelpCommand.Metadata.Description ?? string.Empty;

            var titleScale = 1.0f;
            var titlePosition = new Vector2(left + Config.ConsoleTextPadding, top + Config.ConsoleMessagesInterval);
            var titleSize = Config.ConsoleFont.MeasureString(title) * titleScale;
            spriteBatch.DrawString(Config.ConsoleFont, title, titlePosition, Color.White, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);
            
            var data = Model.HelpCommand.Metadata.Data;
            var dataSize = new Vector2();
            IDataRenderer renderer = null;
            if (data != null)
            {
                if (RenderersProvider.IsRegistered(data.GetType()))
                {
                    renderer = RenderersProvider.GetRenderer(data.GetType());
                    dataSize = renderer.Measure(data,new Vector2(width, width));
                }
            }
            var rightSpacing = Config.ConsoleTextPadding + dataSize.X;
            
            var descriptionLines = WrapTextByLines(Config.ConsoleFont, description, width - Config.ConsoleTextPadding - rightSpacing, out var descriptionSize);
            var lineHeight = descriptionSize.Y / descriptionLines.Count;
            var totalLinesHeight = (lineHeight + Config.ConsoleMessagesInterval) * descriptionLines.Count;

            if (renderer != null)
            {
                var realDataSize = renderer.Measure(data, new Vector2(dataSize.X, totalLinesHeight));
                var x = left + width - realDataSize.X;
                var y = top + titleSize.Y + Config.ConsoleTextPadding + Config.ConsoleMessagesInterval;
                renderer.Draw(data, gameTime, spriteBatch, new Rectangle(new Vector2(x, y).ToPoint(), realDataSize.ToPoint()));
            }
            
            var linePosition = new Vector2(left + Config.ConsoleTextPadding, top + titleSize.Y + Config.ConsoleTextPadding + Config.ConsoleMessagesInterval);
            foreach (var line in descriptionLines)
            {
                spriteBatch.DrawString(Config.ConsoleFont, line, linePosition, Color.LightGray);
                linePosition.Y += lineHeight + Config.ConsoleMessagesInterval;
            }
            
            _commandHelpSize = new Vector2(width, linePosition.Y + Config.ConsoleTextPadding);
        }

        private IReadOnlyCollection<string> WrapTextByLines(SpriteFont font, string text, float availableWidth, out Vector2 totalSize)
        {
            var words = text.Split(' ')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s));

            var result = new List<string>();
            var line = string.Empty;
            totalSize = new Vector2();
            var lastSize = new Vector2();
            foreach (var word in words)
            {
                var newLine = string.IsNullOrEmpty(line) ? line + word : line + ' ' + word;
                var size = font.MeasureString(newLine);
                if (size.X < availableWidth)
                {
                    line = newLine;
                    lastSize = size;
                    continue;
                }

                if (string.IsNullOrEmpty(line))
                {
                    result.Add(newLine);
                    totalSize = new Vector2(
                        Math.Max(totalSize.X, size.X),
                        totalSize.Y + size.Y);
                    lastSize = size;
                    continue;
                }
                
                result.Add(line);
                totalSize = new Vector2(
                    Math.Max(totalSize.X, lastSize.X),
                    totalSize.Y + lastSize.Y);
                lastSize = size;
                line = word;
            }

            if (!string.IsNullOrWhiteSpace(line))
            {
                result.Add(line);
                totalSize = new Vector2(
                    Math.Max(totalSize.X, lastSize.X),
                    totalSize.Y + lastSize.Y);
            }

            return result;
        }
        
        private RenderTarget2D CreateRenderTarget()
        {
            return new RenderTarget2D(GraphicsDevice, (int)Config.DefaultWidth, (int)Config.DefaultHeight * 2);
        }
    }
}