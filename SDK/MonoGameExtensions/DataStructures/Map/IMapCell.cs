namespace MonoGameExtensions.DataStructures
{
    public interface IMapCell<out TPoint> where TPoint : struct
    {
        TPoint GetPointOnMap();
    }
}