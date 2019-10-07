using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.Client.SunGeneration
{
	public class Consistency
	{
		public IReadOnlyList<MoleculeConsistency> Consistences { get; }



		public Consistency(params MoleculeConsistency[] consistences)
		{
			Consistences = consistences;
		}

		public static Consistency FromAtoms(params AtomConsistency[] consistences)
		{
			
		}
	}
}
