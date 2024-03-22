namespace MonoGameExtensions.DataStructures
{
    public interface IBoundedGrid<TPoint, TCell> : IGrid<TPoint, TCell>, IBoundedSpace<TPoint> where TCell : IGridCell<TPoint> where TPoint : struct
    {
        
    }
}