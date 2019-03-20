namespace GameData
{
	public class PassingParams
	{
		public bool Passable { get; }

		public bool Flyable { get; }

		public bool Soarable { get; }

		public bool Swimable { get; }

		public PassingParams(bool passable, bool flyable, bool soarable, bool swimable)
		{
			Passable = passable;
			Flyable = flyable;
			Soarable = soarable;
			Swimable = swimable;
		}

		public static PassingParams Ground()
		{
			return new PassingParams(true, true, true, false);
		}

		public static PassingParams Water()
		{
			return new PassingParams(false, true, true, true);
		}

		public static PassingParams DefaultLet()
		{
			return new PassingParams(false, true, false, false);
		}
	}
}
