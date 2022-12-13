using System.Threading.Tasks;

namespace BoardPlatform.Data.Repositories
{
    public interface IOnBoardWidgetProvider
    {
        Task<IWidget> FindWidgetAsync(IToken widgetToken);

        Task AddWidget(IWidget widget);

        Task RemoveWidget(IToken token);

        Task UpdateWidget(IToken token, IRawWidgetData rawWidgetData);

        Task<IWidget[]> GetAllWidgets();
    }
}