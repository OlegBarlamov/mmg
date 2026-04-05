using X4World.Maps;

namespace Atom.Client.Services
{
    public interface IDetailsGeneratorProvider
    {
        IDetailsGenerator GetGenerator(IGeneratorTarget target);
    }
}