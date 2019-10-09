using System;
using System.Collections.Generic;
using System.Linq;
using Atom.Client.Particles;

namespace Atom.Client.SunGeneration
{
	public class MoleculeConsistency
	{
		public Molecule Particle { get; }

        public float AmountPercents { get; }

		public MoleculeConsistency(Molecule particle, float amountPercents)
		{
			Particle = particle;
		    AmountPercents = amountPercents;
		}

	    public static IReadOnlyList<MoleculeConsistency> CreateFromAtomsConsistences(IReadOnlyList<AtomConsistency> atomConsistences)
	    {
	        var allAtomTypes = atomConsistences.Select(consistency => consistency.Particle).ToArray();
	        var allAvailableMolekules = MoleculeFactory.CreateAllFromAtom(allAtomTypes);

	        return allAvailableMolekules.Select(molecule =>
	                new MoleculeConsistency(molecule, CalculateAmountPercent(molecule, atomConsistences)))
	            .ToArray();
	    }

	    private static float CalculateAmountPercent(Molecule molecule, IReadOnlyList<AtomConsistency> atomConsistences)
	    {
	        return 1;
	        throw new NotImplementedException();
	    }
        
	}
}
