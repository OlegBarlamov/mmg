using Atom.Client.Particles;

namespace Atom.Client.SunGeneration
{
	public class MoleculeConsistency
	{
		public Molecule Particle { get; }

		public float Density { get; }

		public MoleculeConsistency(Molecule particle, float density)
		{
			Particle = particle;
			Density = density;
		}
	}
}
