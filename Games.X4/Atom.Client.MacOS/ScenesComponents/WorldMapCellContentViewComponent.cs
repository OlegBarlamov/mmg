using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.MacOS.Components
{
    public sealed class WorldMapCellContentViewComponent : RenderableView<WorldMapCellContent>
    {
        private readonly FramedBoxComponentDataModel _childBoxModel;

        public WorldMapCellContentViewComponent(WorldMapCellContent dataModel)
        {
            SetDataModel(dataModel);

            _childBoxModel = new FramedBoxComponentDataModel
            {
                Color = Color.Pink,
                GraphicsPassName = "Render_Grouped",
                Position = DataModel.GetWorldPosition(),
                Scale = DataModel.Size,
            };
            
            BoundingBox = new BoundingBox(DataModel.GetWorldPosition() - DataModel.Size / 2, DataModel.GetWorldPosition() + DataModel.Size / 2);
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);

            AddChild(_childBoxModel);
        }
    }
}