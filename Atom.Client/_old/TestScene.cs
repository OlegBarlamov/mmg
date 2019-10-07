//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Atom.Client.Logging;
//using Atom.Client.Services;
//using FrameworkSDK.Logging;
//using FrameworkSDK.MonoGame.Mvc;
//using Logging;
//using Microsoft.Extensions.Logging;
//using Microsoft.Xna.Framework;

//namespace Atom.Client
//{
//	internal class TestScene : Scene
//	{
//		public ILogger Logger { get; }
//		public IConsoleService ConsoleService { get; }
//		private Random _r = new Random(Guid.NewGuid().GetHashCode());

//		private AtomNew[] AtomTypes = new[] {
//			new AtomNew(1, 1), new AtomNew(1, 0),
//			new AtomNew(2, 2), new AtomNew(1, 2), new AtomNew(2, 1),
//			new AtomNew(3, 3), new AtomNew(1, 3), new AtomNew(3, 1),
//			new AtomNew(5, 5), new AtomNew(2, 5), new AtomNew(5, 2),
//			new AtomNew(8, 8), new AtomNew(4, 8), new AtomNew(8, 4),
//			new AtomNew(13, 13), new AtomNew(8, 13), new AtomNew(13, 8),
//			new AtomNew(21, 21), new AtomNew(15, 21), new AtomNew(21, 15),
//		};

//		private Molekulus[] Result;

//		public TestScene(ILogFactory log)
//			: base("my_test_scene")
//		{
//			Logger = log.CreateLogger("atom");
//			var sortedTypes = AtomTypes.OrderByDescending(@new => @new.CanCount).ToArray();
//			var resultMolekulus = sortedTypes.Select(atom =>
//			{
//				var a = atom.Clone();
//				var molekulus = new Molekulus(atom.ToString(), 0, atom.MaxCountOnLayerNumber - atom.External, a);
//				return molekulus;
//			}).ToList();

//			bool added = true;
//			while (added)
//			{
//				added = false;
//				var newMolekulus = new List<Molekulus>();
//				foreach (var atomType in sortedTypes)
//				{
//					var atom = atomType.Clone();

//					foreach (var existsMol in resultMolekulus)
//					{
//						var newMol = MolekulusFactory.TryCombine(existsMol, atom);
//						if (newMol != null && !resultMolekulus.Contains(newMol) && !newMolekulus.Contains(newMol))
//						{
//							added = true;
//							newMolekulus.Add(newMol);
//						}
//					}
//				}

//				resultMolekulus.AddRange(newMolekulus);
//			}

			
//			Result = resultMolekulus.OrderBy(molekulus => molekulus.Atoms.Count).ToArray();

//			//var cubes = new List<TestCubeModel>();

//			//cubes = GenerateAllFromAtoms(
//			//	new AtomElement(1, 0),
//			//	new AtomElement(8, 0)
//			//);
//			//var maxCount = 9999;

//			//int index = 0;
//			//for (int x = 0; x < 120 && maxCount > cubes.Count; x++)
//			//{
//			//	for (int y = 0; y < 90 && maxCount > cubes.Count; y++)
//			//	{
//			//		var pos = new Point(3 + (3 + 10) * x, 3 + (3 + 10) * y);
//			//		cubes.Add(Generate(pos, x, y));
//			//	}
//			//}

//			//foreach (var cube in cubes)
//			//{
//			//	AddView(cube);
//			//}
//		}

//		protected override void OnOpened()
//		{
//			base.OnOpened();

//			var str = string.Join(Environment.NewLine, Result.Select(molekulus => molekulus.ToString()));
//			Logger.Warning(str);
//		}
//	}
//}

