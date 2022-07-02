using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MonoGameExtensions.DataStructures
{
    public class DictionaryBasedGrid<TPoint, TCell> : IGrid<TPoint, TCell> where TPoint : struct where TCell : class, IGridCell<TPoint>
    {
        protected IReadOnlyDictionary<TPoint, TCell> Cells => _cells;
        private readonly ConcurrentDictionary<TPoint, TCell> _cells;

        public DictionaryBasedGrid(IReadOnlyDictionary<TPoint, TCell> initialMap)
        {
            _cells = new ConcurrentDictionary<TPoint, TCell>(initialMap);
        }
        
        public DictionaryBasedGrid()
            :this(new Dictionary<TPoint, TCell>())
        {
        }

        public virtual TCell GetCell(TPoint point)
        {
            if (_cells.TryGetValue(point, out var cell))
                return cell;
            
            return null;
        }

        public virtual void SetCell(TPoint point, TCell data)
        {
            _cells.AddOrUpdate(point, data, (p, cell) => data);
        }

        protected IEnumerable<KeyValuePair<TPoint, TCell>> GetCells()
        {
            return _cells;
        }
    }
}