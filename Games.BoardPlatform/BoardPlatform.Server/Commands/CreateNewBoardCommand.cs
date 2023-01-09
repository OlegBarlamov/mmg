using System;
using System.Threading.Tasks;
using BoardPlatform.Data;
using BoardPlatform.Data.Repositories;
using BoardPlatform.Data.Tokens;
using Console.Core.Commands;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace BoardPlatform.Server.Commands
{
    [RegisterConsoleCommand]
    public class CreateNewBoardCommand : FixedTypedExecutableConsoleCommand
    {
        public IBoardRepository BoardRepository { get; }
        public ITokensFactory TokensFactory { get; }
        public ILogger<CreateNewBoardCommand> Logger { get; }
        public override string Text { get; } = "create_board";
        public override string Description { get; }

        public CreateNewBoardCommand([NotNull] IBoardRepository boardRepository, [NotNull] ITokensFactory tokensFactory,
            [NotNull] ILogger<CreateNewBoardCommand> logger)
        {
            BoardRepository = boardRepository ?? throw new ArgumentNullException(nameof(boardRepository));
            TokensFactory = tokensFactory ?? throw new ArgumentNullException(nameof(tokensFactory));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync()
        {
            var token = TokensFactory.CreateBoardToken();
            await BoardRepository.AddBoard(new Board(token)).ConfigureAwait(false);
            Logger.LogWarning($"Board created: {token.GetId()}");
        }
    }
}