using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BoardPlatform.Data.Repositories
{
    public interface IWidgetRepository
    {
        Task<IWidget> FindWidgetAsync(IToken boardToken, IToken widgetToken);

        Task AddWidget(IToken boardToken, IWidget widget);

        Task RemoveWidget(IToken boardToken, IToken token);

        Task UpdateWidget(IToken boardToken, IToken token, IRawWidgetData rawWidgetData);

        Task<IEnumerable<IWidget>> GetAllWidgetsAsync(IToken boardToken, CancellationToken cancellationToken);
    }
}