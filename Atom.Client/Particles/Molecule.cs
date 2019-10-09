using System.Collections.Generic;
using System.Linq;

namespace Atom.Client.Particles
{
	public class Molecule
	{
        public Atom[] Atoms { get; }

        public float Volume { get; }

        public float CineticEnergy { get; }

	    public Molecule(IEnumerable<Atom> atoms, int electronsCount)
	    {
	        Atoms = atoms.ToArray();

	        Volume = Atoms.Sum(atom => atom.Volume);

	        CineticEnergy = Atoms.Length;
	    }
	}
}
