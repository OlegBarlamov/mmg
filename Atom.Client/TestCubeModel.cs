using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Atom.Client
{
	public class LightWaveSpectr
	{
		public float FromLength;
		public float ToLength;
	}

	public class LightWave
	{
		public LightWaveSpectr[] Lights { get; }

		public LightWave(IEnumerable<LightWaveSpectr> lights)
		{
			Lights = lights.ToArray();
		}
	}

	public class AtomElement
	{
		public float Mass;

		public Vector3 Position;

		public AtomElement(float mass, float size, Vector3 position)
		{
			Mass = mass;
			Position = position;
		}
	}


	public class TestCubeModel
	{
		public Point Position { get; }

		private AtomElement[] Elements { get; }

	    private float _minDistance;

		public TestCubeModel(Point position, [NotNull] AtomElement[] elements)
		{
			Elements = elements ?? throw new ArgumentNullException(nameof(elements));
			Position = position;

		    _minDistance = elements.Min(e1 =>
		        elements.Where(e2 => e2 != e1).Min(e => Vector3.Distance(e.Position, e1.Position)));

		}

		public LightWave LightInput(LightWave lightWave)
		{
			var light = lightWave.Lights.First();

			var max = MaxSize * 10 * 1.5;
			var from = light.FromLength;
			var to = Math.Min(max, light.ToLength);

			var result = new List<float>();

			//поглащает


			return new LightWave(result.Select(f => new LightWaveSpectr
			{
				FromLength = f,
				ToLength = f
			}));
		}
	}
}
