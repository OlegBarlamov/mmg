using System;
using Epic.Battle.Controllers;
using Epic.Data.Battle;
using FrameworkSDK.Game.Views;
using Microsoft.Xna.Framework;

namespace Epic.Battle.Views
{
    internal class BattleFieldView : View<BattleField, BattleFieldController>
    {
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Console.WriteLine(DataModel.Size);
        }
    }
}
