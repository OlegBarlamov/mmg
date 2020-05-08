using System.Collections.Generic;

namespace TablePlatform.Data
{
    public interface ITableGameDescriptor
    {
        IReadOnlyCollection<ICanvasCardMetaType> CardTypes { get; }
        
        IReadOnlyCollection<ICanvasCard> InitialPosition { get; }
    }
}