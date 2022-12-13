using BoardPlatform.Data.Repositories;

namespace BoardPlatform.Data
{
    public interface IBoard : IReferenceable
    {
        IOnBoardWidgetProvider GetWidgetsProvider();
    }
}