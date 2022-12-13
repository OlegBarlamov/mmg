namespace BoardPlatform.Data
{
    public interface ICanvasObject
    {
        IPosition2D GetPosition();

        ISize2D GetSize();
    }
}