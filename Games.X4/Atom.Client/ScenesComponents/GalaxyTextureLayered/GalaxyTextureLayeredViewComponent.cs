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
        private const float TiltAngle = 0.1f;
        private const float LayerSpacing = 0.03f; // fraction of DiskRadius

        private static readonly Matrix[] LayerTilts =
        {
            Matrix.CreateRotationX(-TiltAngle) * Matrix.CreateRotationZ(TiltAngle),
            Matrix.Identity,
            Matrix.CreateRotationZ(-TiltAngle) * Matrix.CreateRotationX(TiltAngle),
        };

        private static readonly float[] LayerOffsets = { -1f, 0f, 1f };

        public GalaxyTextureLayeredViewComponent([NotNull] GalaxyTextureLayeredViewModel3D viewModel)
            : base(CreateMeshes(), viewModel)
        {
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);

            var textures = DataModel.AggregatedData.LayerTextures;
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
            var baseRotation = DataModel.Rotation;
            var diskRadius = DataModel.AggregatedData.DiskRadius;
            var normal = Vector3.TransformNormal(Vector3.Up, baseRotation);
            var spacing = diskRadius * LayerSpacing;

            for (int i = 0; i < Meshes.Count; i++)
            {
                var mesh = (FixedSimpleMesh)Meshes[i];
                mesh.SetPosition(DataModel.Position + normal * (LayerOffsets[i] * spacing));
                mesh.Scale = DataModel.Scale;
                mesh.Rotation = LayerTilts[i] * baseRotation;
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

        private static IRenderableMesh[] CreateMeshes()
        {
            return new IRenderableMesh[]
            {
                new FixedSimpleMesh(new PlaneGeometry()),
                new FixedSimpleMesh(new PlaneGeometry()),
                new FixedSimpleMesh(new PlaneGeometry()),
            };
        }
    }
}
