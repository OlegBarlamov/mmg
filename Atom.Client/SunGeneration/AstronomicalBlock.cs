using System;
using System.Collections.Generic;
using System.Linq;

namespace Atom.Client.SunGeneration
{
	public class AstronomicalBlock
	{
        public float OuterPressureForce;

	    public float DispersionForce;

	    public float Mass = 1;

	    private static readonly float BlockVolume = Helpers.CubeVolume(Constants.MacroBlockSide);

        public static AstronomicalBlock CreateFromConsitensy(IReadOnlyList<MoleculeConsistency> moleculeConsistences, float destinyPercent)
	    {
            var a = new AstronomicalBlock();
	        a.DispersionForce = CalculateDispersionForce(moleculeConsistences, destinyPercent);
	        return a;
	    }

	    private static float CalculateDispersionForce(IReadOnlyList<MoleculeConsistency> moleculeConsistences, float destinyPercent)
	    {
	        var sumEnergy = moleculeConsistences.Sum(consistency => consistency.Particle.CineticEnergy * consistency.AmountPercents);
	        var energy = sumEnergy * destinyPercent * BlockVolume;

            //TODO тут нужно учесть размер молекул. BlockVolume может сократиться.
            var freeSpaceVolume = (1 - destinyPercent) * BlockVolume;

            // 2E^2/R^2
	        return (float)(2 * Math.Pow(energy, 2) / Math.Pow(freeSpaceVolume, 2));
	    }


    }
}
