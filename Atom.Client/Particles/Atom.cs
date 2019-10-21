using System;

namespace Atom.Client.Particles
{
	public class Atom
	{
		public int Protons { get; }
		public int Neytrons { get; }

	    public float Volume {get;}

        public float Mass { get; }

	    public Atom(int protons, int neytrons)
	    {
	        Protons = protons;
	        Neytrons = neytrons;
            Volume = Helpers.Volume(Constants.ProtonQuantsRadius) * Protons +
                     Helpers.Volume(Constants.NeytronQuantsRadius) * Neytrons;

	        Mass = (Protons + Neytrons) * 1;
	    }
	}
}
