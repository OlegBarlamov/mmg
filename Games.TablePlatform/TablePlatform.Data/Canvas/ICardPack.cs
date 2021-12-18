using NetExtensions;
using NetExtensions.Collections;

namespace TablePlatform.Data
{
    public interface ICardPack : IGameObject, IStackApi<ICanvasCard>
    {
        int Count { get; }
        ICanvasCardMetaType CardsMetaType { get; }
    }
}