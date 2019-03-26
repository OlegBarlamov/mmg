using System;
using FrameworkSDK;
using FrameworkSDK.Helpers;
using FrameworkSDK.Services;
using HeroesData.Battle;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroesRendering.Battle
{
	public class BattleFieldCellView : IView
	{
		private BattleFieldCell Model { get; }

		private IGraphicsService GraphicsService { get; }


		private readonly VertexPositionColor[] _vertices;
		private readonly int[] _indexes;

		public BattleFieldCellView([NotNull] BattleFieldCell model, [NotNull] IGraphicsService graphicsService)
		{
			Model = model ?? throw new ArgumentNullException(nameof(model));
			GraphicsService = graphicsService ?? throw new ArgumentNullException(nameof(graphicsService));

			_vertices = CreateVertices(Model.FieldPosition.ToVector3FromXZ(0));
			_indexes = CreateIndexes();
		}

		public void Render(GameTime gameTime)
		{
			var graphicDevices = GraphicsService.GraphicsDevice;

			graphicDevices.DrawUserIndexedPrimitives(
				PrimitiveType.TriangleList,
				_vertices,
				0,
				_vertices.Length,
				_indexes,
				0,
				2);
		}

		public void Update(GameTime gameTime)
		{

		}

		private static VertexPositionColor[] CreateVertices(Vector3 worldPositionLeftTopCorner)
		{
			var z = new Vector3(0,0,1);
			var x = new Vector3(1,0,0);

			return new[]
			{
				new VertexPositionColor(worldPositionLeftTopCorner, Color.Blue),
				new VertexPositionColor(worldPositionLeftTopCorner - z, Color.Blue),
				new VertexPositionColor(worldPositionLeftTopCorner - z + x, Color.Blue),
				new VertexPositionColor(worldPositionLeftTopCorner + x, Color.Blue)
			};
		}

		private static int[] CreateIndexes()
		{
			return new[]
			{
				3, 1, 2,
				3, 0, 1
			};
		}
	}
}
 