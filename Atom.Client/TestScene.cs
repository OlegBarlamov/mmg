using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;

namespace Atom.Client
{
	internal class TestScene : Scene
	{
		private Random _r = new Random(Guid.NewGuid().GetHashCode());

		public TestScene()
			: base("my_test_scene")
		{
			var cubes = new List<TestCubeModel>();

			cubes = GenerateAllFromAtoms(
				new AtomElement(1, 0),
				new AtomElement(8, 0)
			);
			//var maxCount = 9999;

			//int index = 0;
			//for (int x = 0; x < 120 && maxCount > cubes.Count; x++)
			//{
			//	for (int y = 0; y < 90 && maxCount > cubes.Count; y++)
			//	{
			//		var pos = new Point(3 + (3 + 10) * x, 3 + (3 + 10) * y);
			//		cubes.Add(Generate(pos, x, y));
			//	}
			//}

			foreach (var cube in cubes)
			{
				AddView(cube);
			}
		}

		private List<TestCubeModel> GenerateAllFromAtoms(params AtomElement[] atoms)
		{
			int size = 30;
			int width = 100;

			Point GetPosFromIndex(int i)
			{
				var x = i % width;
				var y = i / width;
				return new Point(x, y);
			}

			int index = 0;
			int count = 1;
			while (true)
			{
				for (int i = 1; i <= count; i++)
				{
					
				}
			}
		}

		private TestCubeModel GenerateWater(Point pos, int x, int y)
		{
			return new TestCubeModel(pos, new []
			{
				new AtomElement(WorldConstants.SpinEnergy * 1, 1),
				new AtomElement(WorldConstants.SpinEnergy * 1, 1),
				new AtomElement(WorldConstants.SpinEnergy * 8, 2),
			});
		}

		private TestCubeModel Generate(Point pos, int x, int y)
		{
			var atoms = new List<AtomElement>();
			var count = (x + 1) / 2 + 1;
			for (int i = 0; i<count;i++)
				atoms.Add(GenerateAtom(x,y));

			return new TestCubeModel(pos, atoms.ToArray());
		}

		private AtomElement GenerateAtom(int x, int y)
		{
			var spin = y + 1;
			var energy = spin * WorldConstants.SpinEnergy;
			var electronsCount = _r.Next(spin + 1);
			return new AtomElement(energy, electronsCount);
		}

		private Vector3 GenerateVector()
		{
			return new Vector3(_r.Next(0, 9), _r.Next(0, 9), _r.Next(0, 9));
		}
	}
}
