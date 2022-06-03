namespace MonoGameExtensions.Map
{
    public interface IMapCell<out TPoint> where TPoint : struct
    {
        TPoint GetPointOnMap();
    }
}