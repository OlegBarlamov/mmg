using System;
using System.Collections.Generic;
using System.Linq;
using Atom.Client.Services;
using Atom.Client.SunGeneration;
using JetBrains.Annotations;

namespace Atom.Client
{
	[UsedImplicitly]
	internal class TestConsoleApp : ConsoleApp
	{
		public TestConsoleApp(IConsoleService console)
			: base(console)
		{
		}

		public override void Run()
		{
		    var pAtom = new Particles.Atom(1, 0);
            
            var star = new Star(3, new []{new AtomConsistency(pAtom, 1)}, 0.8f);

		    while (true)
		    {
                var o = star.Output();
                Console.WriteLine(o);

		        Console.ReadLine();

                star.Update();
		    }
        }
	}

    internal class Star
    {
        private readonly List<AstronomicalBlock> _blocks = new List<AstronomicalBlock>();

        public Star(int size, IReadOnlyList<AtomConsistency> atomConsistency, float density)
        {
            for (int i = 0; i < size; i++)
            {
                var consistency = MoleculeConsistency.CreateFromAtomsConsistences(atomConsistency);
                var block = AstronomicalBlock.CreateFromConsitensy(consistency, density);
                if (i > 0)
                {
                    var previousBlocks = _blocks.Take(i);
                    var sumMass = previousBlocks.Sum(b => b.Mass);
                    var force = sumMass * 1;

                    block.OuterPressureForce = force;
                }
                _blocks.Add(block);
            }
        }

        public void Update()
        {

        }

        public string Output()
        {
            var output = string.Empty;
            foreach (var block in _blocks)
            {
                if (!string.IsNullOrWhiteSpace(output))
                    output += "--";
                output += $"[M={block.Mass};D={block.DispersionForce};P={block.OuterPressureForce}]";
            }

            return output;
        }
    }
}
