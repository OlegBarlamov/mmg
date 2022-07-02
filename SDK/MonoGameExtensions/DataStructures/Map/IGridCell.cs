namespace MonoGameExtensions.DataStructures
{
    public interface IGridCell<out TPoint> where TPoint : struct
    {
        TPoint GetPointOnMap();
    }
}