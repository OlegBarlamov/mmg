using System.Collections.Generic;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public abstract class SingleMeshComponent<TData, TController> : View<TData, TController> where TController : IController
    {
        public override IReadOnlyList<string> GraphicsPassNames
        {
            get
            {
                if (!_initialized)
                {
                    Initialize();
                }

                return _graphicsPassNames;
            }
        }

        public override IReadOnlyDictionary<string, IReadOnlyList<IRenderableMesh>> MeshesByPass
        {
            get
            {
                if (!_initialized)
                {
                    Initialize();
                }

                return _meshesByPass;
            }
        }

        public override BoundingBox? BoundingBox => BoundingBoxInternal;
        
        protected abstract BoundingBox BoundingBoxInternal { get; }

        protected abstract IRenderableMesh Mesh { get; }

        protected abstract string SingleGraphicsPassName { get; }
        
        private readonly string[] _graphicsPassNames = new string[1];
        private readonly Dictionary<string, IReadOnlyList<IRenderableMesh>> _meshesByPass = new Dictionary<string, IReadOnlyList<IRenderableMesh>>();
        private readonly IRenderableMesh[] _meshes = new IRenderableMesh[1];

        private bool _initialized;
        
        private void Initialize()
        {
            _initialized = true;

            _graphicsPassNames[0] = SingleGraphicsPassName;
            _meshes[0] = Mesh;
            _meshesByPass[SingleGraphicsPassName] = _meshes;
        }
    }

    public abstract class SingleMeshComponent<TData> : SingleMeshComponent<TData, EmptyController>
    {
    }
}