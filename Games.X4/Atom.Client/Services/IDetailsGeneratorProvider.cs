using X4World.Generation;

namespace Atom.Client.Services
{
    public interface IDetailsGeneratorProvider
    {
        IDetailsGenerator GetGenerator(IGeneratorTarget target);
    }
}