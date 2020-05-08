using NetExtensions;

namespace TablePlatform.Data
{
    public interface ICardPack : IGameObject, IStackApi<ICanvasCard>
    {
        int Count { get; }
        ICanvasCardMetaType CardsMetaType { get; }
    }
}