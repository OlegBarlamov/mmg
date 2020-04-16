namespace Atom.Client.SunGeneration
{
	public class AtomConsistency
	{
		public Particles.Atom Particle { get; }

		public float AmountPercent { get; }

	    public AtomConsistency(Particles.Atom atomType, float amount)
	    {
	        Particle = atomType;
	        AmountPercent = amount;
	    }
	}
}
