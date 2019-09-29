using System;
using System.Collections.Generic;
using System.Linq;
using Atom.Client.Services;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;

namespace Atom.Client
{
	public class TestCubeView : View<TestCubeModel>
	{
		private static readonly Wave Sun = new Wave((int)Constants.MinWaveEnergy, (int)Constants.MaxWaveEnergy);

		private IGameHeart GameHeart { get; }

		private Texture2D _texture;

		private ILogger Logger;

		public TestCubeView([NotNull] IGameHeart gameHeart, ILoggerFactory loggerFactory)
		{
			GameHeart = gameHeart ?? throw new ArgumentNullException(nameof(gameHeart));
			Logger = loggerFactory.CreateLogger("CUBE");
		}

		public override void Draw(GameTime gameTime, IDrawContext context)
		{
			base.Draw(gameTime, context);

			if (_texture == null)
				_texture = GameHeart.GraphicsDeviceManager.GraphicsDevice.GetTextureDiffuseColor(Color.White);
			
			//if (DataModel.BufferColor == null)
			//{
				var newLigthWave = DataModel.LightInput(Sun, new Wave(10000,13000), out var isBlack, out var isAlpha).ToArray();

			Color color;
			if (isBlack)
				color = Color.Black;
			else if (isAlpha)
				color = Color.DimGray;
			else
				color = GetColorFromLightWave(newLigthWave);
				
			if (!isBlack && color == Color.Black)
			{
				color = Color.FromNonPremultiplied(0, 0, 0, 0);
			}
				//var lightLog = string.Join(Environment.NewLine, newLigthWave.Select(w => $"{w.From}-{w.To}"));
				//Logger.Warning(lightLog);
			DataModel.BufferColor = color;
			//}

			context.Draw(_texture, new Rectangle(DataModel.Position, new Point(10, 10)), DataModel.BufferColor.Value);
		}

		private static float _visibleFrom = float.MaxValue;
		private static float _visibleTo = float.MinValue;

		private Color GetColorFromLightWave(IEnumerable<Wave> waves)
		{
			var resultColor = new Vector3();

			//обрезать до видимого диапазона

			foreach (var wave in waves)
			{
				for (var e = wave.From; e <= wave.To; e++)
				{
					var waveLength = Constants.EnergyToWaveLength(e);
					var normalizedWavelength = NormalizeWaveLength(waveLength, Constants.MinWaveLength, Constants.MaxWaveLength, -2400f, 2400);

					//Logger.Error(normalizedWavelength.ToString("F0"));

					if (normalizedWavelength < 380 || normalizedWavelength > 780)
						continue;

					var color = NormalizedWaveLengthToRGB(normalizedWavelength);
					if (color != Vector3.Zero)
					{
						if (e < _visibleFrom)
							_visibleFrom = e;
						if (e > _visibleTo)
							_visibleTo = e;
					}

					if (color.X > resultColor.X)
						resultColor.X = color.X;
					if (color.Y > resultColor.Y)
						resultColor.Y = color.Y;
					if (color.Z > resultColor.Z)
						resultColor.Z = color.Z;
				}
			}

			return Color.FromNonPremultiplied(new Vector4(resultColor / 255, 1));
		}

		private static Vector3 AvarageNonZero(IEnumerable<Vector3> vectors)
		{
			return new Vector3(
				AvarageNonZero(vectors.Select(vector3 => vector3.X)),
				AvarageNonZero(vectors.Select(vector3 => vector3.Y)),
				AvarageNonZero(vectors.Select(vector3 => vector3.Z)));
		}

		private static float AvarageNonZero(IEnumerable<float> values)
		{
			var nonZero = values.Where(f => f > 0).ToArray();
			if (nonZero.Length < 1)
				return 0;

			return nonZero.Sum() / nonZero.Length;
		}

		private static double NormalizeWaveLength(double waveL, double minWaveL, double maxWaveL, double newMinWaveL, double newMaxWaveL)
		{
			var relative = waveL - minWaveL;
			var sourcePercent = relative / (maxWaveL - minWaveL);
			return newMinWaveL + sourcePercent * (newMaxWaveL - newMinWaveL);
		}

		private Vector3 NormalizedWaveLengthToRGB(double wavelength)
		{
			double Gamma = 1;
			double IntensityMax = 255;

			double factor;
			double Red, Green, Blue;

			if ((wavelength >= 380) && (wavelength < 440))
			{
				Red = -(wavelength - 440) / (440 - 380);
				Green = 0.0;
				Blue = 1.0;
			}
			else if ((wavelength >= 440) && (wavelength < 490))
			{
				Red = 0.0;
				Green = (wavelength - 440) / (490 - 440);
				Blue = 1.0;
			}
			else if ((wavelength >= 490) && (wavelength < 510))
			{
				Red = 0.0;
				Green = 1.0;
				Blue = -(wavelength - 510) / (510 - 490);
			}
			else if ((wavelength >= 510) && (wavelength < 580))
			{
				Red = (wavelength - 510) / (580 - 510);
				Green = 1.0;
				Blue = 0.0;
			}
			else if ((wavelength >= 580) && (wavelength < 645))
			{
				Red = 1.0;
				Green = -(wavelength - 645) / (645 - 580);
				Blue = 0.0;
			}
			else if ((wavelength >= 645) && (wavelength < 781))
			{
				Red = 1.0;
				Green = 0.0;
				Blue = 0.0;
			}
			else
			{
				Red = 0.0;
				Green = 0.0;
				Blue = 0.0;
			};

			// Let the intensity fall off near the vision limits

			if ((wavelength >= 380) && (wavelength < 420))
			{
				factor = 0.3 + 0.7 * (wavelength - 380) / (420 - 380);
			}
			else if ((wavelength >= 420) && (wavelength < 701))
			{
				factor = 1.0;
			}
			else if ((wavelength >= 701) && (wavelength < 781))
			{
				factor = 0.3 + 0.7 * (780 - wavelength) / (780 - 700);
			}
			else
			{
				factor = 0.0;
			};


			var vector = new Vector3
			{
				X = Red == 0.0 ? 0 : (int)Math.Round(IntensityMax * Math.Pow(Red * factor, Gamma)),
				Y = Green == 0.0 ? 0 : (int)Math.Round(IntensityMax * Math.Pow(Green * factor, Gamma)),
				Z = Blue == 0.0 ? 0 : (int)Math.Round(IntensityMax * Math.Pow(Blue * factor, Gamma)),
			};

			return vector;
		}
	}
}
