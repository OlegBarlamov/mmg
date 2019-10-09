using System;

namespace Atom.Client
{
    public static class Helpers
    {
        public static float Volume(float radius, int dimension = Constants.SpaceDimension)
        {
            return (float)(Math.PI * Math.Pow(radius, dimension));
        }

        public static float CubeVolume(float side, int dimension = Constants.SpaceDimension)
        {
            return (float)Math.Pow(side, dimension);
        }
    }
}
