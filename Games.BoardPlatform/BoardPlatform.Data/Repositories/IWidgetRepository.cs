using System.Threading.Tasks;

namespace BoardPlatform.Data.Repositories
{
    public interface IWidgetRepository
    {
        Task<IWidget> FindWidgetAsync(IToken boardToken, IToken widgetToken);

        Task AddWidget(IToken boardToken, IWidget widget);

        Task RemoveWidget(IToken boardToken, IToken token);

        Task UpdateWidget(IToken boardToken, IToken token, IRawWidgetData rawWidgetData);

        Task<IWidget[]> GetAllWidgets(IToken boardToken);
    }
}