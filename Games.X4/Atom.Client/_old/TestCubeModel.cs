//using System;
//using System.Collections.Generic;
//using System.Linq;
//using JetBrains.Annotations;
//using Microsoft.Xna.Framework;

//namespace Atom.Client
//{
//	public class Wave
//	{
//		public int From;
//		public int To;

//		public Wave(int from, int to)
//		{
//			From = from;
//			To = to;
//		}
//	}

//	public class WaveSpectr
//	{
//		public List<int> _quants = new List<int>();

//		private List<Wave> _calculatedWaves;

//		public void Add(int energy)
//		{
//			_quants.Add(energy);
//		}

//		public IEnumerable<Wave> GetWaves()
//		{
//			if (_calculatedWaves != null)
//				return _calculatedWaves;

//			_calculatedWaves = new List<Wave>();
//			_quants.Sort();

//			var lastQuant = 0;
//			var from = 0;
//			var to = 0;
//			foreach (var q in _quants)
//			{
//				if (from < 1)
//				{
//					from = q;
//					lastQuant = q;
//					continue;
//				}

//				if (q == lastQuant)
//				{
//					to = q;
//					continue;
//				}

//				if (q - lastQuant == 1)
//				{
//					to = q;
//					lastQuant = q;
//				}
//				else
//				{
//					var wave = new Wave(from, lastQuant);
//					_calculatedWaves.Add(wave);
//					from = q;
//					lastQuant = q;
//					to = q;
//				}
//			}

//			if (to > 0)
//			{
//				var wave = new Wave(from, to);
//				_calculatedWaves.Add(wave);
//			}

//			return _calculatedWaves;
//		}
//	}

//	public class AtomElement
//	{
//		public float MaxEnergy;

//		public int Spin;

//		public int ElectronsCount;

//		public AtomElement(int spin, int electronsCount)
//		{
//			Spin = spin;
//			MaxEnergy = spin * WorldConstants.SpinEnergy;
//			ElectronsCount = electronsCount;
//		}

//		public float GetFreeEnergy()
//		{
//			var electronsEnergy = ElectronsCount * WorldConstants.ElectronEnergy;
//			return MaxEnergy - electronsEnergy;
//		}

//		public AtomElement Clone()
//		{
//			return new AtomElement(Spin, 0);
//		}
//	}


//	public class TestCubeModel
//	{
//		public Point Position { get; }

//		public Color? BufferColor;

//		private AtomElement[] Elements { get; }

//		private WaveSpectr _calculatedSpectr;

//		private bool _isBlack;
//		private bool _isAplpha;

//		public TestCubeModel(Point position, [NotNull] AtomElement[] elements)
//		{
//			Elements = elements ?? throw new ArgumentNullException(nameof(elements));
//			Position = position;
//		}

//		public IEnumerable<Wave> LightInput(Wave inputWave, Wave visibleLight, out bool isBlack, out bool isAlpha)
//		{
//			isBlack = _isBlack;
//			isAlpha = _isAplpha;
//			if (_calculatedSpectr != null)
//				return _calculatedSpectr.GetWaves();

//			bool? isBlackNull = null;
//			bool? isAlphaNull = null;

//			var freeEnergy = Elements.Sum(element => element.GetFreeEnergy());

//			float quants = 0;
//			_calculatedSpectr = new WaveSpectr();

//			for (float w = inputWave.From; w <= inputWave.To; w++)
//			{
//				if (w >= visibleLight.From && w <= visibleLight.To)
//				{
//					if (isBlackNull == null)
//						isBlackNull = true;
//				}

//				quants += w;
//				if (quants < freeEnergy)
//					continue;

//				//кушает сколько смог накопить
//				var d = quants - freeEnergy;
//				quants = 0;

//				//остальное испускает
//				if (d > 0)
//				{
//					if (d >= visibleLight.From && d <= visibleLight.To)
//					{
//						isBlackNull = false;
//						isAlphaNull = false;
//					}
//					_calculatedSpectr.Add((int) d);
//				}

//				//пока кушает, не может поглощать.
//				w += freeEnergy / Elements.Length;
//			}

//			_calculatedSpectr.Add((int)quants);

//			isBlack = isBlackNull == true;
//			isAlpha = isAlphaNull != false && isBlackNull == null;

//			_isBlack = isBlack;
//			_isAplpha = isAlpha;

//			return _calculatedSpectr.GetWaves();
//		}
//	}
//}
