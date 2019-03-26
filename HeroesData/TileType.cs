namespace HeroesData
{
	public abstract class TileType
	{
		public abstract string Name { get; }

		public abstract PassingParams PassingParams { get; }
	}

	public class GrassTileType : TileType
	{
		public override string Name { get; } = "Трава";
		public override PassingParams PassingParams { get; } = PassingParams.Ground();
	}
}
