namespace MonoGameExtensions.DataStructures
{
    public interface IGrid<in TPoint, TCell> where TPoint : struct where TCell : IGridCell<TPoint>
    {
        TCell GetCell(TPoint point);

        void SetCell(TPoint point, TCell data);
    }
}