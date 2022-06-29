using System;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;

namespace FrameworkSDK.MonoGame.Graphics.RenderableComponents
{
    public abstract class RenderablePrimitive<TData, TController> : SingleMeshComponent<TData, TController> where TData : ViewModel3D where TController : IController
    {
        protected RenderablePrimitive(IRenderableMesh mesh, TData viewModel)
            : base(mesh, viewModel.GraphicsPassName)
        {
            Mesh.CopyWorldParameters(viewModel);
            Mesh.Material = viewModel.MeshMaterial;
            viewModel.PlacementChanged += DataModelOnPlacementChanged;
            
            SetDataModel(viewModel);
        }

        protected override void OnDestroy()
        {
            DataModel.PlacementChanged -= DataModelOnPlacementChanged;
            
            base.OnDestroy();
        }

        private void DataModelOnPlacementChanged(object sender, EventArgs e)
        {
            Mesh.CopyWorldParameters(DataModel);
        }
    }

    public abstract class RenderablePrimitive<TData> : RenderablePrimitive<TData, EmptyController>
        where TData : ViewModel3D
    {
        protected RenderablePrimitive(IRenderableMesh mesh, TData viewModel) : base(mesh, viewModel)
        {
        }
    }
}