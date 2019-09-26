using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetExtensions;

namespace MonoGameExtensions
{
	public static class TextureGenerator
	{
		public static Texture2D GetTextureDiffuseColor(this GraphicsDevice graphicsDevice, Color color)
		{
			var texture = GetEmptyTexture(graphicsDevice, 1, 1);
			SetDataToTexture(texture, color);
			return texture;
		}


		/// <summary>
		/// Gets the texture with the gradient color.
		/// </summary>
		/// <param name="color1">The color1.</param>
		/// <param name="color2">The color2.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		/// <param name="angle">The angle between the gradient and Y in degree.</param>
		/// <param name="offset">The offset.</param>
		/// <returns></returns>
		//public static Texture2D GetTextureGradientColor(Color color1, Color color2, int width, int height, float angle, float offset = 0)
		//{
		//	var texture = GetEmptyTexture(height, width);

		//	angle = angle % 360;
		//	if (angle >= 180)
		//	{
		//		Code.Swap(ref color1, ref color2);
		//		angle = angle - 180;
		//	}

		//	var line1 = Line2D.FromPointAngle(new PointF((float)width / 2, (float)height / 2), MathHelper.ToRadians(angle));

		//	var top = new PointF(0, 0);
		//	var bot = new PointF(width - 1, height - 1);
		//	if (angle > 90)
		//	{
		//		top = new PointF(width - 1, 0);
		//		bot = new PointF(0, height - 1);
		//	}

		//	var beginPerpLine = Line2D.FromNormalAndPoint(line1, top);
		//	var endPerpLine = Line2D.FromNormalAndPoint(line1, bot);

		//	var begin = line1.GetIntersection(beginPerpLine);
		//	var end = line1.GetIntersection(endPerpLine);

		//	var distance = new Vector2(begin.X, begin.Y) - new Vector2(end.X, end.Y);
		//	var distance1 = distance.Length() * (1 - offset) / 2;
		//	var distance2 = distance.Length() - distance1;

		//	var diffrenceColor = (color2.ToVector4() - color1.ToVector4()) / 2;
		//	var dColor1 = diffrenceColor / distance1;
		//	var dColor2 = diffrenceColor / distance2;

		//	//НЕ ТРОГАТЬ!!!
		//	var arrayColor = new Color[width, height];
		//	for (int i = 0; i < width; i++)
		//		for (int j = 0; j < height; j++)
		//		{
		//			var currentPerpLine = Line2D.FromNormalAndPoint(line1, new PointF(i, j));
		//			var current = line1.GetIntersection(currentPerpLine);

		//			Vector2 d = begin.ToVector2() - current.ToVector2();
		//			if (d.Length() < distance1)
		//			{
		//				arrayColor[i, j] = Color.FromNonPremultiplied(color1.ToVector4() + dColor1 * d.Length());
		//				continue;
		//			}
		//			if (Math.Abs(d.Length() - distance1) > float.Epsilon)
		//			{
		//				arrayColor[i, j] = Color.FromNonPremultiplied(color2.ToVector4() - dColor2 * (distance.Length() - d.Length()));
		//				continue;
		//			}
		//			arrayColor[i, j] = (Math.Abs(distance2) < float.Epsilon) ? color2 : color1;
		//		}

		//	SetDataToTexture(texture, arrayColor);

		//	return texture;
		//}

		private static Texture2D GetEmptyTexture(GraphicsDevice graphicsDevice, int width, int height)
		{
			return new Texture2D(graphicsDevice, width, height);
		}

		/// <summary>
		/// Sets the data to target texture.
		/// </summary>
		/// <param name="targetTexture">The target texture.</param>
		/// <param name="data">pixels data.</param>
		private static void SetDataToTexture(Texture2D targetTexture, Color[,] data)
		{
			var dataWidth = data.GetLength(0);
			var dataHeight = data.GetLength(1);

			if (targetTexture.Width != dataWidth || targetTexture.Height != dataHeight)
			{
				throw new Exception(
					string.Format(
						"failed to apply the data to the texture, the size of the data ({0}) is not comparable with the size of the texture ({1}).",
						dataWidth + 'x' + dataHeight, targetTexture.Width + 'x' + targetTexture.Height));
			}

			var data1D = Code.ConvertMassive2To1(data);
			targetTexture.SetData(data1D);
		}

		/// <summary>
		/// Sets the data to taget texture. Fill texture equals horizontal lines of pixels.
		/// </summary>
		/// <param name="targetTexture">The target texture.</param>
		/// <param name="data">The horizontal line of pixels.</param>
		private static void SetDataToTextureHorizontal(Texture2D targetTexture, Color[] data)
		{
			int dataLenght = data.Length;

			if (targetTexture.Width != dataLenght)
			{
				throw new Exception(
					string.Format(
						"failed to apply the data to the texture, the size of the data ({0}) is not comparable with the size of the texture ({1}).",
						dataLenght + 'x' + targetTexture.Height, targetTexture.Width + 'x' + targetTexture.Height));
			}

			var resData = new Color[targetTexture.Width * targetTexture.Height];
			for (int i = 0; i < resData.Length; i++)
				resData[i] = data[i % dataLenght];
			targetTexture.SetData(resData);
		}

		/// <summary>
		/// Sets the data to taget texture. Fill texture equals vertical lines of pixels.
		/// </summary>
		/// <param name="targetTexture">The target texture.</param>
		/// <param name="data">The vertical line of pixels.</param>
		private static void SetDataToTextureVertical(Texture2D targetTexture, Color[] data)
		{
			int dataLenght = data.Length;

			if (targetTexture.Height != dataLenght)
			{
				throw new Exception(
					string.Format(
						"failed to apply the data to the texture, the size of the data ({0}) is not comparable with the size of the texture ({1}).",
						targetTexture.Width + 'x' + dataLenght, targetTexture.Width + 'x' + targetTexture.Height));
			}

			var resData = new Color[targetTexture.Width * targetTexture.Height];
			for (int i = 0; i < resData.Length; i++)
				resData[i] = data[i / dataLenght];
			targetTexture.SetData(resData);
		}

		/// <summary>
		/// Fill target texture in diffuse color.
		/// </summary>
		/// <param name="targetTexture">The target texture.</param>
		/// <param name="color">The color.</param>
		private static void SetDataToTexture(Texture2D targetTexture, Color color)
		{
			var resData = new Color[targetTexture.Width * targetTexture.Height];
			for (int i = 0; i < resData.Length; i++)
				resData[i] = color;
			targetTexture.SetData(resData);
		}
	}
}