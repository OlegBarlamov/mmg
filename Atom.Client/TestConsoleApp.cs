using System;
using Atom.Client.Services;
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
			
		}
	}
}
