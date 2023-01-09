using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BoardPlatform.Data.Tokens;
using JetBrains.Annotations;

namespace BoardPlatform.Data.Repositories
{
    public class SimpleInMemoryWidgetRepository : IWidgetRepository
    {
        private ITokensParser TokensParser { get; }
        private static readonly ConcurrentDictionary<string, BoardWidgetCollection> WidgetsByBoards = new ConcurrentDictionary<string, BoardWidgetCollection>();

        public SimpleInMemoryWidgetRepository([NotNull] ITokensParser tokensParser)
        {
            TokensParser = tokensParser ?? throw new ArgumentNullException(nameof(tokensParser));
        }
        
        public Task<IWidget> FindWidgetAsync(IToken boardToken, IToken widgetToken)
        {
            var widgetData = GetBoard(boardToken)[
                widgetToken.GetId()];
            var widget = Widget.FromRawWidgetData(widgetToken, boardToken, widgetData);
            return Task.FromResult(widget);
        }

        public Task AddWidget(IToken boardToken, IWidget widget)
        {
            GetBoard(boardToken).AddOrUpdate(widget.GetToken().GetId(), widget.GetRawData(), (s, widgetData) => widget.GetRawData());
            return Task.CompletedTask;
        }

        public Task RemoveWidget(IToken boardToken, IToken token)
        {
            GetBoard(boardToken).TryRemove(token.GetId(), out _);
            return Task.CompletedTask;
        }

        public Task UpdateWidget(IToken boardToken, IToken token, IRawWidgetData rawWidgetData)
        {
            GetBoard(boardToken)[token.GetId()] = rawWidgetData;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<IWidget>> GetAllWidgetsAsync(IToken boardToken, CancellationToken cancellationToken)
        {
            var board = GetBoard(boardToken);
            return Task.Factory.StartNew(ReadWidgetsFromBoard, board, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        private IEnumerable<IWidget> ReadWidgetsFromBoard(object state)
        {
            var boardWidgetCollection = (BoardWidgetCollection) state;
            foreach (var rawWidgetData in boardWidgetCollection)
            {
                yield return Widget.FromRawWidgetData(TokensParser.FromWidgetId(rawWidgetData.Key), boardWidgetCollection.BoardToken, rawWidgetData.Value);
            }
        }

        private static ConcurrentDictionary<string, IRawWidgetData> GetBoard(IToken board)
        {
            return WidgetsByBoards.GetOrAdd(board.GetId(), s => new BoardWidgetCollection(board));
        }
        
        private class BoardWidgetCollection : ConcurrentDictionary<string, IRawWidgetData>
        {
            public IToken BoardToken { get; }

            public BoardWidgetCollection(IToken boardToken)
            {
                BoardToken = boardToken;
            }
        }
    }
}