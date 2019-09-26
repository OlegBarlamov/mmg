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
		public float Size;

		public Vector2 Position;

		public AtomElement(float mass, float size, Vector2 position)
		{
			Mass = mass;
			Size = size;
			Position = position;
		}
	}


	public class TestCubeModel
	{
		public Point Position { get; }

		private AtomElement[] Elements { get; }

		private float MaxSize { get; }

		private float AvarageDistance { get; }

		public TestCubeModel(Point position, [NotNull] AtomElement[] elements)
		{
			Elements = elements ?? throw new ArgumentNullException(nameof(elements));
			Position = position;

			MaxSize = Elements.Average(element => element.Size);

			var totalTotalDistance = 0f;
			foreach (var element1 in elements)
			{
				var totalDistance = 0f;
				foreach (var element2 in elements)
					totalDistance += Vector2.Distance(element1.Position, element2.Position);

				var avarage = totalDistance / elements.Length;
				totalTotalDistance += avarage;
			}
			AvarageDistance = totalTotalDistance / elements.Length;
		}

		public LightWave LightInput(LightWave lightWave)
		{
			var light = lightWave.Lights.First();

			var max = MaxSize * 10 * 1.5;
			var from = light.FromLength;
			var to = Math.Min(max, light.ToLength);

			var result = new List<float>();

			var delta = AvarageDistance * 3.5f;
			if (delta < 1)
				delta = 1;

			//отражает
			for (var i = 0f; i <= to; i += delta)
			{
				if (i >= from)
					result.Add(i);
			}

			return new LightWave(result.Select(f => new LightWaveSpectr
			{
				FromLength = f,
				ToLength = f
			}));
		}
	}
}
