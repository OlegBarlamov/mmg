namespace MonoGameExtensions.DataStructures
{
    public interface IBoundedSpace<TSpace> where TSpace : struct
    {
        TSpace GetLowerBound();
        TSpace GetUpperBound();
        bool ContainsPoint(TSpace point);
    }
}