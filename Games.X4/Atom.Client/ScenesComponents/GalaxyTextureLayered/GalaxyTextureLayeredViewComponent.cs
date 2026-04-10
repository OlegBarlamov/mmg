using System;
using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class GalaxyTextureLayeredViewComponent : MultiMeshPrimitive<GalaxyTextureLayeredViewModel3D, GalaxyTextureLayeredController>
    {
        private const float TiltAngle = 0.05f;
        private const float LayerSpacing = 0.02f;

        public GalaxyTextureLayeredViewComponent([NotNull] GalaxyTextureLayeredViewModel3D viewModel)
            : base(CreateMeshes(), viewModel)
        {
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);

            var aggData = DataModel.AggregatedData;
            var starViewModel = new GalaxyAsPointViewModel3D(
                DataModel.Position,
                aggData.GalaxyColor,
                0.5f,
                aggData.DiskRadius);
            AddChild(starViewModel);

            var textures = aggData.LayerTextures;
            for (int i = 0; i < textures.Length; i++)
            {
                var index = i;
                textures[i].TextureChanged += () => OnLayerTextureChanged(index);
                OnLayerTextureChanged(i);
            }
        }

        protected override void OnDetached(SceneBase scene)
        {
            base.OnDetached(scene);
        }

        protected override void UpdateMeshTransforms()
        {
            var count = Meshes.Count;
            var baseRotation = DataModel.Rotation;
            var diskRadius = DataModel.AggregatedData.DiskRadius;
            var normal = Vector3.TransformNormal(Vector3.Up, baseRotation);
            var spacing = diskRadius * LayerSpacing;

            for (int i = 0; i < count; i++)
            {
                var mesh = (FixedSimpleMesh)Meshes[i];
                var offset = count > 1 ? (i - (count - 1) / 2f) : 0f;
                var tilt = count > 1 ? GetLayerTilt(i, count) : Matrix.Identity;
                mesh.SetPosition(DataModel.Position + normal * (offset * spacing));
                mesh.Scale = DataModel.Scale;
                mesh.Rotation = tilt * baseRotation;
            }
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            return new BoundingBox(DataModel.Position - DataModel.Scale / 2, DataModel.Position + DataModel.Scale / 2);
        }

        private void OnLayerTextureChanged(int layerIndex)
        {
            var texture = DataModel.AggregatedData.LayerTextures[layerIndex].Texture;
            if (texture != null)
            {
                Meshes[layerIndex].Material = new TextureMaterial(texture);
            }
        }

        private static Matrix GetLayerTilt(int index, int count)
        {
            var t = (index - (count - 1) / 2f) / ((count - 1) / 2f);
            return Matrix.CreateRotationX(t * -TiltAngle) * Matrix.CreateRotationZ(t * TiltAngle);
        }

        private static IRenderableMesh[] CreateMeshes()
        {
            var meshes = new IRenderableMesh[GalaxyTextureLayeredAggregatedData.LayerCount];
            for (int i = 0; i < meshes.Length; i++)
                meshes[i] = new FixedSimpleMesh(new PlaneGeometry());
            return meshes;
        }
    }
}
