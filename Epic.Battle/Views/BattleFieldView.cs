using System;
using Epic.Battle.Controllers;
using Epic.Data.Battle;
using FrameworkSDK.MonoGame.GameStructure.Views;
using FrameworkSDK.MonoGame.Graphics;
using Microsoft.Xna.Framework;

namespace Epic.Battle.Views
{
    public class BattleFieldView : View<BattleField, BattleFieldController>
	{
		public override void Draw(GameTime gameTime, IDrawContext drawContext)
		{
			base.Draw(gameTime, drawContext);

			Console.WriteLine(DataModel.Size);
		}
	}
}