using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace BoardPlatform.Data.Repositories
{
    public class SimpleInMemoryBoardRepository : IBoardRepository
    {
        private static readonly ConcurrentDictionary<string, IBoard> Boards = new ConcurrentDictionary<string, IBoard>();

        public Task<IBoard> FindBoardAsync(IToken token)
        {
            var board = Boards[token.GetId()];
            return Task.FromResult(board);
        }

        public Task AddBoard([NotNull] IBoard board)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            var token = board.GetToken();
            
            Boards.AddOrUpdate(token.GetId(), board, (s, existedBoard) => board);
            
            return Task.CompletedTask;
        }

        public Task RemoveBoard(IToken token)
        {
            Boards.TryRemove(token.GetId(), out _);
            return Task.CompletedTask;
        }

        public Task<bool> BoardExist(IToken token)
        {
            return Task.FromResult(Boards.ContainsKey(token.GetId()));
        }
    }
}