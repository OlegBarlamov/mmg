using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;

namespace Atom.Client
{
	public class TestCubeView : View<TestCubeModel>
	{
		private static readonly LightWave Sun = new LightWave(new []
		{
			new LightWaveSpectr
			{
				FromLength = 10,
				ToLength = 100
			}
		});

		private IGameHeart GameHeart { get; }

		private Texture2D _texture;

		public TestCubeView([NotNull] IGameHeart gameHeart)
		{
			GameHeart = gameHeart ?? throw new ArgumentNullException(nameof(gameHeart));
		}

		public override void Draw(GameTime gameTime, IDrawContext context)
		{
			base.Draw(gameTime, context);

			if (_texture == null)
				_texture = GameHeart.GraphicsDeviceManager.GraphicsDevice.GetTextureDiffuseColor(Color.White);

			var newLigthWave = DataModel.LightInput(Sun);

			var color = GetColorFromLightWave(newLigthWave);
			context.Draw(_texture, new Rectangle(DataModel.Position, new Point(30, 30)), color);
		}

		private static Color GetColorFromLightWave(LightWave lightWave)
		{
			var resultColor = new Vector3();
			var lights = lightWave.Lights;

			foreach (var lightWaveSpectr in lights)
			{
				for (var i = lightWaveSpectr.FromLength; i <= lightWaveSpectr.ToLength; i++)
				{
					var color = WaveLengthToRGB(i * 10);
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

		private static Vector3 WaveLengthToRGB(double wavelength)
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
			else if ((wavelength >= 580) && (wavelength < /*645*/ 650))
			{
				Red = 1.0;
				Green = -(wavelength - /*645*/650) / (/*645*/650 - 580);
				Blue = 0.0;
			}
			else if ((wavelength >= /*645*/650) && (wavelength < /*781*/780))
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
			else if ((wavelength >= 420) && (wavelength < /*701*/700))
			{
				factor = 1.0;
			}
			else if ((wavelength >= /*701*/700) && (wavelength < /*781*/780))
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
