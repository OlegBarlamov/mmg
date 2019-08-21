using System;
using Epic.Battle.Models;
using FrameworkSDK.Game.Scenes;
using JetBrains.Annotations;

namespace Epic.Game
{
    internal class BattleScene : Scene
    {
        public BattleScene([NotNull] BattleModel battleModel)
        {
            if (battleModel == null) throw new ArgumentNullException(nameof(battleModel));
            AddController(battleModel);
        }
    }
}
