using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameworkSDK.Game.Scenes;

namespace FrameworkSDK.Game
{
	internal interface IScenesController : IUpdatable, IDrawable, IDisposable
	{
		bool CanSceneChanged { get; }

		Scene CurrentScene { get; set; }
	}
}
