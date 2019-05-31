﻿using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace HeroesData.Battle
{
	public class BattleFieldCell
	{
		public TileType TileType { get; }

		public Point FieldPosition => ParentField.GetCellPosition(this);

		private BattleField ParentField { get; }

		public BattleFieldCell([NotNull] BattleField parentField, [NotNull] TileType tileType)
		{
			ParentField = parentField ?? throw new ArgumentNullException(nameof(parentField));
			TileType = tileType ?? throw new ArgumentNullException(nameof(tileType));
		}
	}
}