using System;

namespace Epic.Logic.Generator
{
    public interface IRandomProvider
    {
        Random Random { get; }
    }

    public class FixedRandomProvider : IRandomProvider
    {
        public Random Random { get; }

        public FixedRandomProvider(Random random)
        {
            Random = random;
        }
    }
}