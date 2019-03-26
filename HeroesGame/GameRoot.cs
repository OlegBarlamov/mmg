using System;
using FrameworkSDK;
using FrameworkSDK.Services;
using HeroesData.Battle;
using HeroesRendering.Battle;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HeroesGame
{
	[UsedImplicitly]
	class GameRoot : GameBase
	{
		private BattleField _battleField;
		private BattleFieldView _battleFieldView;

		private BasicEffect _effect;
		private RasterizerState _rasterizerState;

		private IViewFactory ViewFactory { get; }

		public GameRoot([NotNull] IViewFactory viewFactory)
		{
			ViewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
			
		}
		
		protected override void Initialize()
		{
			_battleField = BattleField.Generate();
			_battleFieldView = ViewFactory.CreateView<BattleFieldView>(_battleField);

			_rasterizerState = new RasterizerState
			{
				FillMode = FillMode.WireFrame
			};

			base.Initialize();
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			_effect = new BasicEffect(GraphicsDevice);
			_effect.VertexColorEnabled = true;
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			//GraphicsDevice.RasterizerState = _rasterizerState;

			var cameraPosition = new Vector3(0, 30, 0);
			var cameraLookAtVector = new Vector3(0,0,0);
			var cameraUpVector = Vector3.Forward;

			_effect.View = Matrix.CreateLookAt(
				cameraPosition, cameraLookAtVector, cameraUpVector);

			float aspectRatio =
				GraphicsDeviceManager.PreferredBackBufferWidth / (float)GraphicsDeviceManager.PreferredBackBufferHeight;
			float fieldOfView = MathHelper.PiOver4;
			float nearClipPlane = 1;
			float farClipPlane = 200;

			_effect.Projection = Matrix.CreatePerspectiveFieldOfView(
				fieldOfView, aspectRatio, nearClipPlane, farClipPlane);

			foreach (var pass in _effect.CurrentTechnique.Passes)
			{
				pass.Apply();

				_battleFieldView.Render(gameTime);
			}

			base.Draw(gameTime);
		}
	}
}
