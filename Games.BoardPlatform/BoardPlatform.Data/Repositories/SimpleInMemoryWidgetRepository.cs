using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace BoardPlatform.Data.Repositories
{
    public class SimpleInMemoryWidgetRepository : IWidgetRepository
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, IRawWidgetData>> WidgetsByBoards = new ConcurrentDictionary<string, ConcurrentDictionary<string, IRawWidgetData>>();
        
        public Task<IWidget> FindWidgetAsync(IToken boardToken, IToken widgetToken)
        {
            var widgetData = GetBoard(boardToken)[
                widgetToken.GetId()];
            var widget = Widget.FromRawWidgetData(widgetData);
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

        public Task<IWidget[]> GetAllWidgets(IToken boardToken)
        {
            var widgets = GetBoard(boardToken).Values.Select(Widget.FromRawWidgetData).ToArray();
            return Task.FromResult(widgets);
        }

        private static ConcurrentDictionary<string, IRawWidgetData> GetBoard(IToken board)
        {
            return WidgetsByBoards.GetOrAdd(board.GetId(), s => new ConcurrentDictionary<string, IRawWidgetData>());
        }
    }
}