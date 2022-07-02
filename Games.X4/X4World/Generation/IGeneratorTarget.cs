using System.Collections.Generic;

namespace X4World.Generation
{
    public interface IGeneratorTarget
    {
        void SetGeneratedData(IReadOnlyCollection<IWrappedDetails> objects);
    }
}