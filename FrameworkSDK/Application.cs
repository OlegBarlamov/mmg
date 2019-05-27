using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkSDK
{
	public abstract class Application : IApplication
	{
		private readonly GameShell _gameShell;

		public Application()
		{
			_gameShell = new GameShell();
		}
		
		public void Run()
		{
			_gameShell.Run();
		}
	}
}
