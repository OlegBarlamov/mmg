using System;
using Epic.Battle.Models;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace Epic.Game
{
    internal class BattleScene : Scene
    {
        public BattleScene([NotNull] BattleModel battleModel) 
            : base("BattleScene")
        {
            if (battleModel == null) throw new ArgumentNullException(nameof(battleModel));
            AddController(battleModel);
        }
    }
}
