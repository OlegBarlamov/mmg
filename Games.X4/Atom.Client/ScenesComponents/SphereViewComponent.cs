using System;
using Atom.Client.Resources;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;
using X4World;
using X4World.Objects;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class SphereViewComponent : RenderableView<PlanetSystemFarthest>
    {
        public MainResourcePackage ResourcePackage { get; }
        
        public SphereViewComponent([NotNull] PlanetSystemFarthest model, [NotNull] MainResourcePackage resourcePackage,
            [NotNull] ICamera3DProvider camera3DProvider, IDisplayService displayService)
        {
            ResourcePackage = resourcePackage ?? throw new ArgumentNullException(nameof(resourcePackage));
            
            SetDataModel(model);
            
            // Mesh.Material = new TextureMaterial(ResourcePackage.A);
            // Mesh.Position = DataModel.GetWorldPosition();
            // Mesh.Scale = new Vector3(DataModel.AggregatedData.Power * 0.5f);
        }

        // protected override BoundingBox? ConstructBoundingBox()
        // {
        //     var size = new Vector3(DataModel.AggregatedData.Power * 0.5f);
        //     return new BoundingBox(DataModel.GetWorldPosition() -  size / 2, DataModel.GetWorldPosition() + size / 2);
        // }
    }
}