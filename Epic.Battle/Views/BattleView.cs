using Epic.Battle.Controllers;
using Epic.Battle.Models;
using FrameworkSDK.Game.Scenes;
using FrameworkSDK.Game.Views;

namespace Epic.Battle.Views
{
    internal class BattleView : View<BattleModel, BattleController>
    {
        protected override void Initialize(Scene scene)
        {
            base.Initialize(scene);

            AddChild(DataModel.Field);
        }
    }
}
