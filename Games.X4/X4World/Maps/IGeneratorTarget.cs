using System.Collections.Generic;

namespace X4World.Maps
{
    public interface IGeneratorTarget
    {
        void SetGeneratedData(IReadOnlyCollection<IWrappedDetails> objects);
    }
}
