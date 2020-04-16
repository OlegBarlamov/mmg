using System;
using System.Collections.Generic;
using System.Linq;
using Console.Core;
using Console.Core.Implementations;
using Console.Core.Models;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Console.InGame.Implementation
{
    internal class ConsoleModel : IDisposable
    {
        public event Action<string> UserCommand;

        public float Height { get; }
        public float Width { get; }
        public Vector2 Position { get; }

        public string EnteringCommandText { get; private set; } = string.Empty;

        public int CaretPosition { get; private set; }
        
        public Vector2 VisibleTextOffset { get; private set; }
        public IEnumerable<IRenderingMessage> VisibleMessages => _currentVisibleMessages;

        public bool IsVisibleMessagesChanged { get; private set; }
        
        public IReadOnlyCollection<CommandSuggestion> VisibleCommandSuggestions { get; private set; } =
            Array.Empty<CommandSuggestion>();

        public int VisibleSelectedSuggestionIndex { get; private set; } = -1;
        public bool MoreSuggestionLeft { get; private set; }
        public bool MoreSuggestionRight { get; private set; }
        public bool SuggestionIndexChanged { get; private set; }
        [CanBeNull] public IConsoleCommand HelpCommand { get; private set; }

        public bool IsAllowControl { get; set; }

        [NotNull] private InGameConsoleConfig Config { get; }
        [NotNull] private RenderersProvider RenderersProvider { get; }

        private bool _needUpdateVisibleMessages;
        private bool _scrolledByEnd = true;
        private int _startVisibleIndex;
        private int _lastVisibleIndex;
        private bool _fullPageFilled;
        private bool _needScrollByEnd;
        private IEnumerable<IRenderingMessage> _currentVisibleMessages = Enumerable.Empty<IRenderingMessage>();
        private string _lastEnteringCommandText = string.Empty;
        
        private int _suggestionIndex = -1;
        private int _lastSuggestionIndex = -1;
        private IReadOnlyList<CommandSuggestion> _suggestions = Array.Empty<CommandSuggestion>();

        private float _scrollOffsetY;
        private int _enteredCommandSelectedIndex = -1;

        private float notUsedOffsetY = 0;
        
        private readonly List<IRenderingMessage> _allMessages = new List<IRenderingMessage>();
        private readonly object _allMessagesLocker = new object();
        private readonly InputController _inputController = new InputController();
        private readonly CommandSuggestionsProvider _commandSuggestionsProvider;
        private readonly List<string> _enteredCommands = new List<string>();
        
        public ConsoleModel([NotNull] InGameConsoleConfig config, [NotNull] IConsoleCommandExecutor commandsProvider,
            [NotNull] RenderersProvider renderersProvider)
        {
            if (commandsProvider == null) throw new ArgumentNullException(nameof(commandsProvider));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            RenderersProvider = renderersProvider ?? throw new ArgumentNullException(nameof(renderersProvider));

            Height = config.DefaultHeight;
            Width = config.DefaultWidth;
            Position = config.Position;
            
            _commandSuggestionsProvider = new CommandSuggestionsProvider(commandsProvider);
        }

        public void Dispose()
        {
            UserCommand = null;
        }

        public void OnShow()
        {
            _needScrollByEnd = true;
            _needUpdateVisibleMessages = true;
        }

        public void AddMessages(IEnumerable<IConsoleMessage> messages)
        {
            var newMessages = new List<IRenderingMessage>();
            foreach (var message in messages)
            {
                var formatted= FormatConsoleMessage(message);
                foreach (var formattedMessage in formatted)
                    newMessages.Add(formattedMessage);
            }

            if (!newMessages.Any())
                return;
            
            lock (_allMessagesLocker)
            {
                _allMessages.AddRange(newMessages);
                if (_scrolledByEnd)
                {
                    if (_fullPageFilled)
                        _startVisibleIndex += newMessages.Count;
                    _needUpdateVisibleMessages = true;
                    _needScrollByEnd = true;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            _inputController.Update(gameTime);

            ProcessInputCommandLine(gameTime);
            ProcessSuggestions();
            ProcessHelpCommand();
            ProcessScrolling();
            
            IsVisibleMessagesChanged = _needUpdateVisibleMessages;
            if (_needUpdateVisibleMessages)
            {
                UpdateVisibleMessages();
            }
        }

        private void UpdateVisibleMessages()
        {
            lock (_allMessagesLocker)
            {
                _needUpdateVisibleMessages = false;

                var newVisibleMessages = new List<IRenderingMessage>();
                var totalHeight = 0f;
                var availableHeight = GetAvailableHeight();
                if (_needScrollByEnd)
                {
                    _needScrollByEnd = false;
                    VisibleTextOffset = Vector2.Zero;
                    int i;
                    for (i = _allMessages.Count - 1; i >= 0; i--)
                    {
                        var message = _allMessages[i];
                        totalHeight += message.Size.Y + Config.ConsoleMessagesInterval;

                        newVisibleMessages.Insert(0, message);

                        if (totalHeight > availableHeight)
                        {
                            i--;
                            VisibleTextOffset = new Vector2(0, totalHeight - availableHeight);
                            break;
                        }
                    }

                    _startVisibleIndex = i + 1;
                    _lastVisibleIndex = _allMessages.Count - 1;
                    _scrolledByEnd = true;
                }
                else
                {
                    _startVisibleIndex = ReturnToBounds(_startVisibleIndex, 0, _allMessages.Count - 1);

                    //scrolling
                    if ((int)_scrollOffsetY != 0)
                    {
                        var currentOffset = VisibleTextOffset.Y + _scrollOffsetY;
                        var extraOffset = 0f;
                        if (currentOffset > 0)
                        {
                            while (currentOffset > 0)
                            {
                                if (_startVisibleIndex == _allMessages.Count - 1)
                                {
                                    extraOffset = 0;
                                    break;
                                }
                                extraOffset = currentOffset;

                                var hiddenMessage = _allMessages[_startVisibleIndex];
                                currentOffset -= hiddenMessage.Size.Y;
                                if (currentOffset > 0)
                                    _startVisibleIndex++;
                            }
                        }
                        else
                        {
                            while (currentOffset < 0)
                            {
                                if (_startVisibleIndex == 0)
                                {
                                    extraOffset = 0;
                                    break;
                                }
                                _startVisibleIndex--;
                                var shownMessage = _allMessages[_startVisibleIndex];
                                currentOffset += shownMessage.Size.Y;
                                extraOffset = currentOffset;
                            }
                        }
                        
                        VisibleTextOffset = new Vector2(0, extraOffset);
                    }
                    
                    int i;
                    for (i = _startVisibleIndex; i < _allMessages.Count; i++)
                    {
                        var message = _allMessages[i];
                        totalHeight += message.Size.Y + Config.ConsoleMessagesInterval;

                        newVisibleMessages.Add(message);

                        if (totalHeight > availableHeight + VisibleTextOffset.Y)
                        {
                            i++;
                            break;
                        }
                    }

                    _lastVisibleIndex = i - 1;
                    _scrolledByEnd = i >= _allMessages.Count;
                }

                _fullPageFilled = totalHeight >= availableHeight;
                _currentVisibleMessages = newVisibleMessages;
            }
        }


        public void Clear()
        {
            lock (_allMessagesLocker)
            {
                _allMessages.Clear();
            }

            _startVisibleIndex = 0;
            _needUpdateVisibleMessages = true;
        }

        private void ProcessScrolling()
        {
            _scrollOffsetY = 0;
            const float scrollSpeed = 0.08f; 
            var scrollDelta = _inputController.MouseScrollWheelDelta;
            if (Math.Abs(scrollDelta) < float.Epsilon)
                return;
            
            _scrollOffsetY = -scrollDelta * scrollSpeed;
            
            _needUpdateVisibleMessages = true;
        }
        
        private void ProcessSuggestions()
        {
            if (_lastEnteringCommandText != EnteringCommandText)
            {
                _suggestions = !string.IsNullOrEmpty(EnteringCommandText) 
                    ?_commandSuggestionsProvider.GetSuggestions(EnteringCommandText).ToArray()
                    : Array.Empty<CommandSuggestion>();

                _suggestionIndex = -1;
                _lastSuggestionIndex = -1;
                VisibleSelectedSuggestionIndex = -1;
            }

            if (_suggestionIndex != _lastSuggestionIndex || _lastEnteringCommandText != EnteringCommandText)
            {
                var leftSuggestionIndex = _suggestionIndex - Config.CommandSuggestionsCount / 2;
                var rightSuggestionIndex = _suggestionIndex + Config.CommandSuggestionsCount / 2;
                if (Config.CommandSuggestionsCount % 2 == 0)
                {
                    if (_lastSuggestionIndex < _suggestionIndex)
                    {
                        rightSuggestionIndex--;
                    }
                    else
                    {
                        leftSuggestionIndex++;
                    }
                }

                MoreSuggestionLeft = leftSuggestionIndex > 0;
                if (leftSuggestionIndex < 0)
                    rightSuggestionIndex += -leftSuggestionIndex;

                MoreSuggestionRight = rightSuggestionIndex < _suggestions.Count - 1;
                if (rightSuggestionIndex >= _suggestions.Count)
                {
                    MoreSuggestionRight = false;
                    rightSuggestionIndex = _suggestions.Count - 1;
                    leftSuggestionIndex = rightSuggestionIndex - Config.CommandSuggestionsCount + 1;
                }

                if (leftSuggestionIndex < 0)
                    leftSuggestionIndex = 0;

                VisibleCommandSuggestions = _suggestions
                    .Skip(leftSuggestionIndex)
                    .Take(rightSuggestionIndex - leftSuggestionIndex + 1)
                    .ToArray();

                VisibleSelectedSuggestionIndex = _suggestionIndex - leftSuggestionIndex;
            }

            SuggestionIndexChanged = _suggestionIndex != _lastSuggestionIndex;
            _lastEnteringCommandText = EnteringCommandText;
            _lastSuggestionIndex = _suggestionIndex;
        }

        private void ProcessHelpCommand()
        {
            HelpCommand = null;
            if (_suggestionIndex >= 0)
            {
                HelpCommand = _suggestions[_suggestionIndex].Command;
            }
            else
            {
                if (EnteringCommandText.Length > 0)
                {
                    var firstWord = EnteringCommandText.Split(' ').First();
                    var firstSuggestion = _suggestions.FirstOrDefault();
                    if (firstSuggestion != null && string.Equals(firstWord, firstSuggestion.Command.Text, StringComparison.OrdinalIgnoreCase))
                        HelpCommand = firstSuggestion.Command;
                }
            }
        }

        private void ProcessInputCommandLine(GameTime gameTime)
        {
            var enteredChar = _inputController.GetPressedOnceCharacter(gameTime);
            if (enteredChar != null)
            {
                EnteringCommandText = EnteringCommandText.Insert(CaretPosition, enteredChar.ToString());
                CaretPosition++;
                _enteredCommandSelectedIndex = -1;
            }
            if (_inputController.IsKeyPressedOnceRepeated(gameTime, Keys.Back))
            {
                if (CaretPosition > 0)
                {
                    EnteringCommandText = EnteringCommandText.Remove(CaretPosition - 1, 1);
                    CaretPosition--;
                    _enteredCommandSelectedIndex = -1;
                }
            }
            if (_inputController.IsKeyPressedOnceRepeated(gameTime, Keys.Delete))
            {
                if (_inputController.IsShift)
                {
                    EnteringCommandText = string.Empty;
                    _enteredCommandSelectedIndex = -1;
                }
                if (CaretPosition < EnteringCommandText.Length)
                {
                    EnteringCommandText = EnteringCommandText.Remove(CaretPosition, 1);
                    _enteredCommandSelectedIndex = -1;
                }
            }
            if (_inputController.IsKeyPressedOnceRepeated(gameTime, Keys.Left))
            {
                CaretPosition--;
            }
            if (_inputController.IsKeyPressedOnceRepeated(gameTime, Keys.Right))
            {
                if (CaretPosition == EnteringCommandText.Length && _suggestions.Count == 1)
                {
                    EnteringCommandText = _suggestions[0].Command.Text;
                    CaretPosition = EnteringCommandText.Length;
                    _suggestionIndex = -1;
                    _enteredCommandSelectedIndex = -1;
                }
                else
                    CaretPosition++;
            }
            if (_inputController.IsKeyPressedOnce(gameTime, Keys.Enter))
            {
                if (!string.IsNullOrEmpty(EnteringCommandText) && _suggestionIndex < 0)
                {
                    UserCommand?.Invoke(EnteringCommandText);
                    _enteredCommands.Add(EnteringCommandText);
                    EnteringCommandText = string.Empty;
                    CaretPosition = 0;
                    _needScrollByEnd = true;
                    _needUpdateVisibleMessages = true;
                    _enteredCommandSelectedIndex = -1;
                }

                if (_suggestionIndex >= 0)
                {
                    EnteringCommandText = _suggestions[_suggestionIndex].Command.Text;
                    CaretPosition = EnteringCommandText.Length;
                    _suggestionIndex = -1;
                    _enteredCommandSelectedIndex = -1;
                }
            }

            if (_inputController.IsKeyPressedOnce(gameTime, Keys.Down))
            {
                _suggestionIndex += 1;
            }
            if (_inputController.IsKeyPressedOnce(gameTime, Keys.Tab))
            {
                _suggestionIndex += 1;
                if (_suggestionIndex >= _suggestions.Count)
                    _suggestionIndex = 0;
            }
            if (_inputController.IsKeyPressedOnce(gameTime, Keys.Up))
            {
                if (_suggestionIndex > -1)
                    _suggestionIndex -= 1;
                else
                {
                    if (_enteredCommands.Count > 0)
                    {
                        _enteredCommandSelectedIndex--;
                        if (_enteredCommandSelectedIndex < 0)
                            _enteredCommandSelectedIndex = _enteredCommands.Count - 1;
                        
                        EnteringCommandText = _enteredCommands[_enteredCommandSelectedIndex];
                        CaretPosition = EnteringCommandText.Length;
                        _suggestionIndex = -1;
                    }
                }
            }
            if (_inputController.IsKeyPressedOnce(gameTime, Keys.Escape))
            {
                _suggestionIndex = -1;
            }
            
            _suggestionIndex = ReturnToBounds(_suggestionIndex, -1, _suggestions.Count - 1);
            CaretPosition = ReturnToBounds(CaretPosition, 0, EnteringCommandText.Length);
        }

        private IEnumerable<IRenderingMessage> FormatConsoleMessage(IConsoleMessage message)
        {
            var fullText = message.Message;
            var availableWidth = Width - Config.ConsoleTextPadding * 2;
            var color = GetMessageColor(message);

            var texts = fullText.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            foreach (var text in texts)
            {
                var size = Config.ConsoleFont.MeasureString(text);

                var estimateMessageText = text;
                Vector2? estimateTextSize = size;
                while (estimateMessageText.Length > 0)
                {
                    if (!estimateTextSize.HasValue)
                        estimateTextSize = Config.ConsoleFont.MeasureString(estimateMessageText);
                    
                    if (estimateTextSize.Value.X < availableWidth)
                    {
                        yield return new TextMessage(estimateMessageText, color, estimateTextSize.Value, Config.ConsoleFont);
                        break;
                    }

                    var estimateWidth = estimateTextSize.Value.X;
                    var extraWidth = estimateWidth - availableWidth;
                    var extraPart = extraWidth / estimateWidth;
                    var extraCharactersLength = (int) (text.Length * extraPart);
                    var availableCharactersLength = text.Length - extraCharactersLength;
                    var availableText = text.Substring(0, availableCharactersLength);

                    yield return new TextMessage(availableText, color, estimateTextSize.Value, Config.ConsoleFont);

                    estimateMessageText = text.Substring(availableCharactersLength);
                    estimateTextSize = null;
                }
            }

            if (message.Content != null)
            {
                var contentType = message.Content.GetType();
                if (RenderersProvider.IsRegistered(contentType))
                {
                    var renderer = RenderersProvider.GetRenderer(contentType);
                    var startSize = renderer.Measure(message.Content,
                        new Vector2(Config.DefaultWidth, Config.DefaultHeight));
                    var customDataMessage = new CustomMessage(message.Content, renderer, startSize);
                    yield return customDataMessage;
                }
            }
        }

        private static Color GetMessageColor(IConsoleMessage message)
        {
            switch (message.LogLevel)
            {
                case ConsoleLogLevel.Trace:
                    return Color.White;
                case ConsoleLogLevel.Debug:
                    return Color.LightGray;
                case ConsoleLogLevel.Information:
                    return Color.White;
                case ConsoleLogLevel.Warning:
                    return Color.Yellow;
                case ConsoleLogLevel.Error:
                    return Color.Red;
                case ConsoleLogLevel.Critical:
                    return Color.DarkRed;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private float GetAvailableHeight()
        {
            return Height - Config.ConsoleCommandLineSectionHeight - Config.ConsoleTextPadding * 2;
        }

        private static int ReturnToBounds(int value, int leftBound, int rightBound)
        {
            if (rightBound < leftBound)
                rightBound = leftBound;
            
            if (value < leftBound)
                return leftBound;
            if (value > rightBound)
                return rightBound;
            return value;
        }
    }
}
