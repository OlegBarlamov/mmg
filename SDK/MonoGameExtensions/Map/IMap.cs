namespace MonoGameExtensions.Map
{
    public interface IMap<in TPoint, TCell> where TPoint : struct where TCell : IMapCell<TPoint>
    {
        TCell GetCell(TPoint point);

        void SetCell(TPoint point, TCell data);
    }
}