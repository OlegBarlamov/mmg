using System.Collections.Generic;
using Atom.Client.Particles;

namespace Atom.Client.SunGeneration
{
    public static class MoleculeFactory
    {
        public static IReadOnlyList<Molecule> CreateAllFromAtom(IReadOnlyList<Particles.Atom> atomTypes)
        {
            return new[] {new Molecule(atomTypes, 0)};
        }
    }
}
