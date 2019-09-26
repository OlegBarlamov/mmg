using Atom.Client.Services;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace Atom.Client
{
	[UsedImplicitly]
	internal class AtomApp : GameApplication
	{
		public override Scene CurrentScene { get; } = new TestScene();

		public AtomApp(IConsoleService consoleService)
		{
			consoleService.Show();
		}
	}
}
