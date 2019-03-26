using System;
using FrameworkSDK.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.Implementations
{
	internal class GraphicsService : IGraphicsService
	{
		public GraphicsDeviceManager GraphicsDeviceManager => Application.Game.GraphicsDeviceManager;
		public GraphicsDevice GraphicsDevice => Application.Game.GraphicsDevice;
		public SpriteBatch SpriteBatch => Application.Game.SpriteBatch;

		private ApplicationBase Application { get; }

		public GraphicsService([NotNull] ApplicationBase application)
		{
			Application = application ?? throw new ArgumentNullException(nameof(application));
		}
	}
}
