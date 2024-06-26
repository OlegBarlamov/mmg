using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class SimpleRenderInstancedMeshes<TVertexType> : GraphicsPipelineActionBase
        where TVertexType : struct, IVertexType
    {
        public Effect Effect { get; }
        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }

        private readonly Dictionary<Type, Dictionary<IGraphicComponent, List<IRenderableMesh>>> _meshesByGeometryType = new Dictionary<Type, Dictionary<IGraphicComponent, List<IRenderableMesh>>>();
        
        public SimpleRenderInstancedMeshes([NotNull] string name,
            [NotNull] Effect effect, [NotNull] VertexBuffer vertexBuffer,
            [NotNull] IndexBuffer indexBuffer) : base(name)
        {
            Effect = effect ?? throw new ArgumentNullException(nameof(effect));
            VertexBuffer = vertexBuffer ?? throw new ArgumentNullException(nameof(vertexBuffer));
            IndexBuffer = indexBuffer ?? throw new ArgumentNullException(nameof(indexBuffer));
        }

        public override void OnComponentAttached(IGraphicComponent attachingComponent)
        {
            var meshes = attachingComponent.MeshesByPass[Name];
            foreach (var mesh in meshes)
            {
                var meshGeometryType = mesh.Geometry.GetType();
                if (!_meshesByGeometryType.ContainsKey(meshGeometryType))
                    _meshesByGeometryType.Add(meshGeometryType,
                        new Dictionary<IGraphicComponent, List<IRenderableMesh>>());

                var componentsMap = _meshesByGeometryType[meshGeometryType];
                if (!componentsMap.ContainsKey(attachingComponent))
                    componentsMap.Add(attachingComponent, new List<IRenderableMesh>());
                
                _meshesByGeometryType[meshGeometryType][attachingComponent].Add(mesh);
            }
        }

        public override void OnComponentDetached(IGraphicComponent detachingComponent)
        {
            var meshes = detachingComponent.MeshesByPass[Name];
            foreach (var mesh in meshes)
            {
                var meshGeometryType = mesh.Geometry.GetType();
                _meshesByGeometryType[meshGeometryType][detachingComponent].Remove(mesh);
                if (_meshesByGeometryType[meshGeometryType][detachingComponent].Count == 0)
                {
                    _meshesByGeometryType[meshGeometryType].Remove(detachingComponent);
                    if (_meshesByGeometryType[meshGeometryType].Count == 0)
                    {
                        _meshesByGeometryType.Remove(meshGeometryType);
                    }
                }
            }
        }

        protected override void Dispose()
        {
            base.Dispose();

            foreach (var meshByGeometryType in _meshesByGeometryType)
            {
                meshByGeometryType.Value.Clear();
            }
            _meshesByGeometryType.Clear();
        }

        private int _meshesIndex;
        private IRenderableMesh _mesh;
        private Dictionary<IGraphicComponent, List<IRenderableMesh>> _meshesByComponents;
        private List<IRenderableMesh> _meshes;
        private IMeshGeometry _geometry;

        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext,
            IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            graphicDeviceContext.RenderContext.GeometryRenderer.SetBuffers(VertexBuffer, IndexBuffer);
            
            // TODO get rid of foreaches here. It leads collection changed exceptinos.
            foreach (var meshesByGeometryName in _meshesByGeometryType)
            {
                _meshesByComponents = meshesByGeometryName.Value;
                foreach (var meshByComponent in _meshesByComponents)
                {
                    if (!IsComponentVisibleByActiveCamera(graphicDeviceContext, meshByComponent.Key))
                        continue;

                    _meshes = meshByComponent.Value;
                    _geometry = _meshes[0].Geometry;
                    
                    graphicDeviceContext.RenderContext.GeometryRenderer.Charge(_geometry);

                    for (_meshesIndex = 0; _meshesIndex < _meshes.Count; _meshesIndex++)
                    {
                        _mesh = _meshes[_meshesIndex];

                        foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                        {
                            ((IEffectMatrices) Effect).World = _mesh.World;
                            _mesh.Material.ApplyToEffect(Effect);
                            
                            pass.Apply();

                            graphicDeviceContext.RenderContext.GeometryRenderer.Render();
                        }
                        
                        graphicDeviceContext.DebugInfoService.IncrementCounter(DebugInfoRenderingVertices, _geometry.GetVertices().Length);
                        graphicDeviceContext.DebugInfoService.IncrementCounter(DebugInfoRenderingMeshes);
                    }
                    
                    graphicDeviceContext.DebugInfoService.IncrementCounter(DebugInfoRenderingComponents);
                }
            }
        }
    }
}