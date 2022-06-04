using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MonoGameExtensions.Map
{
    public class DictionaryBasedMap<TPoint, TCell> : IMap<TPoint, TCell> where TPoint : struct where TCell : class, IMapCell<TPoint>
    {
        private readonly ConcurrentDictionary<TPoint, TCell> _cells;

        public DictionaryBasedMap(IReadOnlyDictionary<TPoint, TCell> initialMap)
        {
            _cells = new ConcurrentDictionary<TPoint, TCell>(initialMap);
        }
        
        public DictionaryBasedMap()
            :this(new Dictionary<TPoint, TCell>())
        {
        }

        public TCell GetCell(TPoint point)
        {
            if (_cells.TryGetValue(point, out var cell))
                return cell;
            
            return null;
        }

        public void SetCell(TPoint point, TCell data)
        {
            _cells.AddOrUpdate(point, data, (p, cell) => data);
        }

        protected IEnumerable<KeyValuePair<TPoint, TCell>> GetCells()
        {
            return _cells;
        }
    }
}