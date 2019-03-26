using FrameworkSDK.Helpers;
using Microsoft.Xna.Framework;

namespace HeroesData.Battle
{
	public class BattleField
	{
		public int Width { get; }
		public int Height { get; }

		public BattleFieldCell[,] Map { get; }

		private BattleField(int width, int height)
		{
			Width = width;
			Height = height;
			Map = new BattleFieldCell[width,height];
		}

		/// <summary>
		/// Less then zero if not exists
		/// </summary>
		public Point GetCellPosition(BattleFieldCell fieldCell)
		{
			var result = new Point(int.MinValue,int.MinValue);
			Map.For((cell, x, y) =>
			{
				if (cell == fieldCell)
				{
					result = new Point(x,y);
					return true;
				}

				return false;
			});
			return result;
		}

		public static BattleField Generate()
		{
			int width = 50; int height = 10;
			var field = new BattleField(width,height);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (x == 0 && y == 0)
						continue;

					if (x == 5 && y == 2)
						continue;

					var tile = new GrassTileType();
					field.Map[x, y] = new BattleFieldCell(field, tile);
				}
			}

			return field;
		}
	}
}
