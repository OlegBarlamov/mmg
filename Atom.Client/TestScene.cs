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

			var maxCount = 9999;

			for (int x = 0; x < 15 && maxCount > cubes.Count; x++)
			{
				for (int y = 0; y < 15 && maxCount > cubes.Count; y++)
				{
					var pos = new Point(10 + (10 + 30) * x, 10 + (10 + 30) * y);
					cubes.Add(Generate(pos));
				}
			}

			foreach (var cube in cubes)
			{
				AddView(cube);
			}
		}

		private TestCubeModel Generate(Point pos)
		{
			var count = _r.Next(3, 90) / 3;
			var atoms = new List<AtomElement>();
			for (int i = 0; i < count; i++)
				atoms.Add(GenerateAtom());

			return new TestCubeModel(pos, atoms.ToArray());
		}

		private AtomElement GenerateAtom()
		{
			return new AtomElement(0, _r.Next(1,10), GenerateVector());
		}

		private Vector2 GenerateVector()
		{
			return new Vector2(_r.Next(0, 9), _r.Next(0, 9));
		}
	}
}
