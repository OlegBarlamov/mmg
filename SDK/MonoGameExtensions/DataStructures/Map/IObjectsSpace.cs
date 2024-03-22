using System.Collections.Generic;

namespace MonoGameExtensions.DataStructures
{
    public interface IObjectsSpace<TSpace, TMapObject> : IBoundedSpace<TSpace>, ICollection<TMapObject> where TSpace : struct where TMapObject : class
    {
        IReadOnlyCollection<TMapObject> GetInRange(TSpace start, TSpace end);
        IReadOnlyCollection<TMapObject> GetInRadius(TSpace center, float radius);
    }
}