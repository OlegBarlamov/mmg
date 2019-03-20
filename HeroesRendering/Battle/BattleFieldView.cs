using System;
using GameData;
using GameData.Battle;
using GameSDK;
using GameSDK.Helpers;
using GameSDK.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace HeroesRendering.Battle
{
	public class BattleFieldView : IView
	{
		private BattleField BattleField { get; }

		private IViewFactory ViewFactory { get; }


		private readonly BattleFieldCellView[,] _battleFieldCellViews;

		public BattleFieldView([NotNull] BattleField battleField, [NotNull] IViewFactory viewFactory)
		{
			BattleField = battleField ?? throw new ArgumentNullException(nameof(battleField));
			ViewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));

			_battleFieldCellViews = CreateCellViews(BattleField.Map, ViewFactory);
		}

		public void Render(GameTime gameTime)
		{
			foreach (var battleFieldCellView in _battleFieldCellViews)
			{
				battleFieldCellView?.Render(gameTime);
			}
		}

		public void Update(GameTime gameTime)
		{
		}

		private static BattleFieldCellView[,] CreateCellViews(BattleFieldCell[,] cells, IViewFactory viewFactory)
		{
			return cells.Select((cell, i, arg3) =>
			{
				if (cell != null)
					return viewFactory.CreateView<BattleFieldCellView>(cell);

				return null;
			});
		}
	}
}
