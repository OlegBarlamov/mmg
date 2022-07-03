using X4World.Generation;

namespace Atom.Client.MacOS.Services
{
    public interface IDetailsGeneratorProvider
    {
        IDetailsGenerator GetGenerator(IGeneratorTarget target);
    }
}