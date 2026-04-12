using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class StarSystemAsPointView : RenderablePrimitive<StarSystemAsPointViewModel3D>
    {
        public StarSystemAsPointView(
            [NotNull] StarSystemAsPointViewModel3D viewModel,
            [NotNull] ICamera3DProvider camera3DProvider)
            : base(new FixedSimpleMesh(CreateGeometry(viewModel, camera3DProvider)), viewModel)
        {
        }

        private static StarSystemAsPointGeometry CreateGeometry(
            StarSystemAsPointViewModel3D viewModel, ICamera3DProvider camera3DProvider)
        {
            return new StarSystemAsPointGeometry(
                viewModel.AggregatedData,
                viewModel.Position,
                () => camera3DProvider.GetActiveCamera().GetPosition());
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            var pos = DataModel.Position;
            var extent = new Vector3(0.1f);
            return new BoundingBox(pos - extent, pos + extent);
        }
    }
}
