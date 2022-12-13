using System.Threading.Tasks;

namespace BoardPlatform.Data.Repositories
{
    public interface IBoardRepository
    {
        Task<IBoard> FindBoardAsync(IToken token);

        Task AddBoard(IBoard board);

        Task RemoveBoard(IToken token);
    }
}