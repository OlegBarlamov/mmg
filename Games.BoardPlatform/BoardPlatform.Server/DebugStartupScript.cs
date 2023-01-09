using System;
using System.Collections.Generic;
using BoardPlatform.Data;
using BoardPlatform.Data.Repositories;
using BoardPlatform.Data.Tokens;
using FrameworkSDK;
using JetBrains.Annotations;

namespace BoardPlatform.Server
{
    public class DebugStartupScript : IAppComponent
    {
        public IBoardRepository BoardRepository { get; }
        public ITokensFactory TokensFactory { get; }
        public IWidgetRepository WidgetRepository { get; }

        public DebugStartupScript([NotNull] IBoardRepository boardRepository, [NotNull] ITokensFactory tokensFactory,
            [NotNull] IWidgetRepository widgetRepository)
        {
            BoardRepository = boardRepository ?? throw new ArgumentNullException(nameof(boardRepository));
            TokensFactory = tokensFactory ?? throw new ArgumentNullException(nameof(tokensFactory));
            WidgetRepository = widgetRepository ?? throw new ArgumentNullException(nameof(widgetRepository));
        }

        public void Dispose()
        {
            
        }

        public void Configure()
        {
            var token = SimpleNumberToken.FakeToken("test_board");
            BoardRepository.AddBoard(new Board(token)).Wait();

            var widgetToken = TokensFactory.CreateWidgetToken();
            WidgetRepository.AddWidget(widgetToken, Widget.FromRawWidgetData(widgetToken, token, DictionaryBasedRawWidgetData.FromDictionary(new Dictionary<string,object>()
            {
                {"position", new Position2D() },
                {"size", new Size2D {Width = 100, Height = 300} }
            })));
        }
    }
}