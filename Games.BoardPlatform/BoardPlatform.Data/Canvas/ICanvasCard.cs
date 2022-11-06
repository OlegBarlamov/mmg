namespace TablePlatform.Data
{
    public interface ICanvasCard : IGameObject
    {
        ICanvasCardType CardType { get; }
    }
}