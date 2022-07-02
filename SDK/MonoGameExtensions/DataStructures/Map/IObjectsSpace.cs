using System.Collections.Generic;

namespace MonoGameExtensions.DataStructures
{
    public interface IObjectsSpace<TSpace, TMapObject> : ICollection<TMapObject> where TSpace : struct where TMapObject : class
    {
        IReadOnlyCollection<TMapObject> GetInRange(TSpace start, TSpace end);
        IReadOnlyCollection<TMapObject> GetInRadius(TSpace center, float radius);
        bool ContainsPoint(TSpace point);
    }
}