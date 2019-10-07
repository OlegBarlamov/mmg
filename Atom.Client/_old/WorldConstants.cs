//using System;

//namespace Atom.Client
//{
//	internal static class WorldConstants
//	{
//		public const int Fps = 120;

//		public const int MatterQuantizationFactor = 4;
//		public const int MicroQuantLayersCount = 4;

//		public const float StandartBlockSize = 1;
//		public const float EnergyQuant = 1;

//		public const float ElectronEnergy = EnergyQuant * 70;
//		public const float SpinEnergy = EnergyQuant * 290;
//	}

//	public static class Constants
//	{
//		public const float MinTsec = 1f / WorldConstants.Fps;

//		public const float C = WorldConstants.StandartBlockSize / MinTsec; // = 120
		
//		public static float PlankDistance { get; } = CalculatePlankDistance(); // ~ 0.00390625


//		public const float MaxWaveLength = C / WorldConstants.EnergyQuant;
//		public static float MinWaveLength { get; } = PlankDistance;

//		public static float MinWaveEnergy { get; } = C / MaxWaveLength;
//		public static float MaxWaveEnergy { get; } = C / MinWaveLength; // 30719.998


//		public static float EnergyToWaveLength(float energy)
//		{
//			return MaxWaveLength - (energy - MinWaveEnergy) / (MaxWaveEnergy - MinWaveEnergy) * (MaxWaveLength - MinWaveLength);
//		}

//		private static float CalculatePlankDistance()
//		{
//			return (float)(WorldConstants.StandartBlockSize / Math.Pow(WorldConstants.MicroQuantLayersCount, WorldConstants.MatterQuantizationFactor));
//		}
//	}

//}
