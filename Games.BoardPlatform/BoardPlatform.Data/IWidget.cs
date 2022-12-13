namespace BoardPlatform.Data
{
    public interface IWidget : ICanvasObject, IBoardsObject, IReferenceable
    {
        IRawWidgetData GetRawData();
    }
}