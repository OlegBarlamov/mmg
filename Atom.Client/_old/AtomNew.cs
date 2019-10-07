//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Atom.Client
//{
//	internal class AtomNew : IEquatable<AtomNew>
//	{
//		public static int[] Layers = new[] {0, 1, 2, 4, 8, 16, 32, 64, 128};

//		public int P;
//		public int N;

//		public int External;

//		public int CanCount;
//		public int LayerNumber;
//		public int MaxCountOnLayerNumber;

//		public bool IsMetal => LayerNumber >= 4 && External * 2 < MaxCountOnLayerNumber;

//		public AtomNew(int p, int n)
//		{
//			P = p;
//			N = n;

//			CanCount = (p + n + 1) / 2;
//			External = CalculateExternals(CanCount, out LayerNumber, out MaxCountOnLayerNumber);
//		}

//		public AtomNew Clone()
//		{
//			return new AtomNew(P, N);
//		}

//		public override string ToString()
//		{
//			return $"{P}P{N}N";
//		}

//		private static int CalculateExternals(int count, out int layerNumber, out int maxCountOnLayerNumber)
//		{
//			for (int i = 1; i <= Layers.Length; i++)
//			{
//				var newCount = count - i;
//				if (newCount == 0)
//				{
//					layerNumber = i;
//					maxCountOnLayerNumber = Layers[layerNumber];
//					return maxCountOnLayerNumber;
//				}

//				if (newCount < 0)
//				{
//					layerNumber = i;
//					maxCountOnLayerNumber = Layers[layerNumber];
//					return count;
//				}

//				count = newCount;
//			}
//			layerNumber = Layers.Length - 1;
//			maxCountOnLayerNumber = Layers[Layers.Length - 1];
//			return count;
//		}

//		public bool Equals(AtomNew other)
//		{
//			if (ReferenceEquals(null, other)) return false;
//			if (ReferenceEquals(this, other)) return true;
//			return P == other.P && N == other.N;
//		}

//		public override bool Equals(object obj)
//		{
//			if (ReferenceEquals(null, obj)) return false;
//			if (ReferenceEquals(this, obj)) return true;
//			if (obj.GetType() != this.GetType()) return false;
//			return Equals((AtomNew) obj);
//		}

//		public override int GetHashCode()
//		{
//			unchecked
//			{
//				return (P * 397) ^ N;
//			}
//		}
//	}

//	internal class Molekulus : IEquatable<Molekulus>
//	{
//		public List<AtomNew> Atoms { get; }
//		public string Name { get; }
//		public int FreePlaces { get; }

//		public int P;

//		public int Used;

//		public int CanCount;

//		public bool IsMetal => Atoms.FirstOrDefault()?.IsMetal == true;

//		public Molekulus(IEnumerable<AtomNew> atoms, string name, int used, int freePlaces)
//		{
//			Atoms = new List<AtomNew>(atoms);
//			Name = name;
//			FreePlaces = freePlaces;

//			P = Atoms.Sum(@new => @new.P);
//			CanCount = Atoms.Sum(@new => @new.CanCount);
//			Used = used;
//		}

//		public Molekulus(string name, int used, int freePlaces, params AtomNew[] atoms) :this(atoms, name, used, freePlaces)
//		{
			
//		}

//		public override string ToString()
//		{
//			return $"{Name}: Q={P - CanCount} - {P - (CanCount - FreePlaces)}; {IsMetal}.";
//		}

//		public bool Equals(Molekulus other)
//		{
//			if (ReferenceEquals(null, other)) return false;
//			if (ReferenceEquals(this, other)) return true;
//			return other.Atoms.Count == Atoms.Count && Atoms.All(@new => other.Atoms.Contains(@new));
//		}

//		public override bool Equals(object obj)
//		{
//			if (ReferenceEquals(null, obj)) return false;
//			if (ReferenceEquals(this, obj)) return true;
//			if (obj.GetType() != this.GetType()) return false;
//			return Equals((Molekulus) obj);
//		}

//		public override int GetHashCode()
//		{
//			return (Atoms != null ? Atoms.GetHashCode() : 0);
//		}
//	}

//	internal static class MolekulusFactory
//	{
//		public static Molekulus TryCombine(AtomNew atom1, AtomNew atom2)
//		{
//			var d = atom1.MaxCountOnLayerNumber - atom1.External;
//			if (atom2.External > d)
//				return null;

//			return new Molekulus($"{atom1}+{atom2}", atom2.External, d - atom2.External, atom1, atom2);
//		}

//		public static Molekulus TryCombine(Molekulus molekulus, AtomNew atom)
//		{
//			var d = molekulus.FreePlaces;
//			if (atom.External > d)
//				return null;

//			return new Molekulus(molekulus.Atoms.Concat(new[] { atom }), $"{molekulus.Name}+{atom}", molekulus.Used + atom.External, d - atom.External);
//		}
//	}
//}
