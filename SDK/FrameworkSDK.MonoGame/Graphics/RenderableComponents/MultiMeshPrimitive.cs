using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.RenderableComponents
{
    public abstract class MultiMeshPrimitive<TData, TController> : RenderableView<TData, TController>
        where TData : ViewModel3D
        where TController : class, IController
    {
        public override IReadOnlyList<string> GraphicsPassNames { get; }
        public override IReadOnlyDictionary<string, IReadOnlyList<IRenderableMesh>> MeshesByPass { get; }
        public override BoundingBox? BoundingBox => CurrentBoundingBox ?? (CurrentBoundingBox = ConstructBoundingBox());

        protected BoundingBox? CurrentBoundingBox;
        protected IReadOnlyList<IRenderableMesh> Meshes { get; }

        protected MultiMeshPrimitive(IRenderableMesh[] meshes, TData viewModel)
        {
            Meshes = meshes;
            foreach (var mesh in meshes)
            {
                mesh.Parent = this;
                mesh.CopyWorldParameters(viewModel);
                mesh.Material = viewModel.MeshMaterial;
            }

            GraphicsPassNames = new[] { viewModel.GraphicsPassName };
            MeshesByPass = new Dictionary<string, IReadOnlyList<IRenderableMesh>>
            {
                { viewModel.GraphicsPassName, meshes }
            };

            viewModel.PlacementChanged += OnPlacementChanged;
            SetDataModel(viewModel);
            UpdateMeshTransforms();
        }

        public void AssignControllerToPrimitive(TController controller)
        {
            if (Controller != null)
                throw new FrameworkMonoGameException(
                    $"Multi-mesh primitive {Name} already has controller assigned: {Controller}");

            SetController(controller);
            controller.SetDataModel(DataModel);
            controller.SetView(this);
        }

        protected override void OnDestroy()
        {
            DataModel.PlacementChanged -= OnPlacementChanged;
            base.OnDestroy();
        }

        protected virtual void UpdateMeshTransforms()
        {
            foreach (var mesh in Meshes)
                mesh.CopyWorldParameters(DataModel);
        }

        protected abstract BoundingBox? ConstructBoundingBox();

        private void OnPlacementChanged(object sender, EventArgs e)
        {
            UpdateMeshTransforms();
            CurrentBoundingBox = ConstructBoundingBox();
        }
    }

    public abstract class MultiMeshPrimitive<TData> : MultiMeshPrimitive<TData, EmptyController>
        where TData : ViewModel3D
    {
        protected MultiMeshPrimitive(IRenderableMesh[] meshes, TData viewModel) : base(meshes, viewModel)
        {
        }
    }
}
