using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.Client
{
    public static class Constants
    {
        public const int Fps = 120;

        public const int SpaceDimension = 3;
        public const int MatterQuantizationBase = 4;
        public const int MatterQuantizationFactor = 2;

        public const int MicroQuantLayersCount = 4 + 1; // 1 фиктивный слой для строения молекулы.
        public const int MacroQuantLayersCount = 4; 

        public const float StandartBlockSize = 1;
        public const float EnergyQuant = 1;


        public const int ProtonQuantsRadius = 10; // подобрано так, чтобы миниально-возможная молекула (из одного атома из одного нейтрона),
        public const int NeytronQuantsRadius = 8; // занимала 1/4 длины ячейки молекулы на слое частиц.
        public const int ElectronQuantsSize = 1; // радиус - половина квантового расстояния. Но он не делим.


        public const float TimeQuantInSeconds = 1f / Fps;

        public const float C = StandartBlockSize / TimeQuantInSeconds; // = 120 стандартных блоков

        public static int MatterQuantsCount = CalculateMatterQuantsCount(); // - Кол-во квантовых расстояний в единичном кубе.
        public static float PlankDistance { get; } = 1 / (float)MacroQuantLayersCount; // - минимально допустимое расстояние. Квант длины.

        public static int MacroBlockSide = CalculateMatterQuantsCount();

        private static int CalculateMatterQuantsCount()
		{
            //TODO проверить
            var factor = (2 * 3 + (MicroQuantLayersCount - 1))*(float)MicroQuantLayersCount / 2;
			return (int)(MatterQuantizationBase * Math.Pow(MatterQuantizationFactor, factor));
		}
    }
}
