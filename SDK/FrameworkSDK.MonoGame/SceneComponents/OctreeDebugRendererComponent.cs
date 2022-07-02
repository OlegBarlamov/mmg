using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class OctreeDebugRendererComponentData<TNodeData> : ViewModel
    {
        public Color Color { get; set; } = Color.White;
        public IOctreeNode<TNodeData> OctreeRootNode { get; }

        public OctreeDebugRendererComponentData([NotNull] IOctreeNode<TNodeData> octreeRootNode)
        {
            OctreeRootNode = octreeRootNode ?? throw new ArgumentNullException(nameof(octreeRootNode));
        }
    }
    
    public class OctreeDebugRendererComponent<TNodeData> : RenderableView<OctreeDebugRendererComponentData<TNodeData>>
    {
        private readonly Dictionary<IOctreeNode<TNodeData>, FramedBoxComponent> _childComponents = new Dictionary<IOctreeNode<TNodeData>, FramedBoxComponent>();

        public OctreeDebugRendererComponent(OctreeDebugRendererComponentData<TNodeData> data)
        {
            SetDataModel(data);
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);
            
            AddBoxesToSceneRecursive(DataModel.OctreeRootNode);
        }

        protected override void OnDetached(SceneBase scene)
        {
            base.OnDetached(scene);
            
            RemoveBoxesFromSceneRecursive(DataModel.OctreeRootNode);
        }

        private void OctreeRootNodeOnNodeSubdivided(IOctreeNode<TNodeData> node)
        {
            node.NodeSubdivided -= OctreeRootNodeOnNodeSubdivided;
            var nodeBox = _childComponents[node];
            RemoveChild(nodeBox);

            foreach (var child in node.Children.Nodes)
            {
                AddBoxesToSceneRecursive(child);
            }
        }

        private void AddBoxesToSceneRecursive(IOctreeNode<TNodeData> node)
        {
            if (!node.Children.IsEmpty)
            {
                foreach (var child in node.Children.Nodes)
                {
                    AddBoxesToSceneRecursive(child);
                }
            }
            else
            {
                node.NodeSubdivided += OctreeRootNodeOnNodeSubdivided;
                var framedBoxData = FramedBoxComponentDataModel.FromBoundingBox(node.BoundingBox);
                framedBoxData.Color = DataModel.Color;
                framedBoxData.GraphicsPassName = DataModel.GraphicsPassName;
                var framedBoxComponent = (FramedBoxComponent)AddChild(framedBoxData);
                _childComponents.Add(node, framedBoxComponent);
            }
        }

        private void RemoveBoxesFromSceneRecursive(IOctreeNode<TNodeData> node)
        {
            if (!node.Children.IsEmpty)
            {
                foreach (var child in node.Children.Nodes)
                {
                    RemoveBoxesFromSceneRecursive(child);
                }
            }
            else
            {
                node.NodeSubdivided -= OctreeRootNodeOnNodeSubdivided;
            }
        }
    }
}