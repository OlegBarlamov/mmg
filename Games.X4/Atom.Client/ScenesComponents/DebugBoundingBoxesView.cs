using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents;
using Microsoft.Xna.Framework;

namespace Atom.Client.Components
{
    public class DebugBoundingBoxesView : View
    {
        private static readonly HashSet<Type> IgnoredTypes = new HashSet<Type>
        {
            typeof(FramedBoxComponentDataModel),
            typeof(FramedBoxWithDiagonalsComponentDataModel),
            typeof(DrawLabelComponentDataModel),
            typeof(LodStatsComponentData),
        };

        private readonly Dictionary<IView, IView> _boxViews = new Dictionary<IView, IView>();
        private readonly string _graphicsPassName;

        public DebugBoundingBoxesView(string graphicsPassName)
        {
            _graphicsPassName = graphicsPassName ?? throw new ArgumentNullException(nameof(graphicsPassName));
        }

        public void TrackView(IView view)
        {
            if (view == this || view.BoundingBox == null || _boxViews.ContainsKey(view))
                return;

            if (IgnoredTypes.Contains(view.DataModel.GetType()))
                return;

            var boxModel = FramedBoxComponentDataModel.FromBoundingBox(view.BoundingBox.Value);
            boxModel.Color = Color.LimeGreen;
            boxModel.GraphicsPassName = _graphicsPassName;
            var boxView = new FramedBoxComponent(boxModel);
            AddChild(boxView);
            _boxViews[view] = boxView;
        }

        public void UntrackView(IView view)
        {
            if (_boxViews.TryGetValue(view, out var boxView))
            {
                RemoveChild(boxView);
                (boxView.DataModel as IDisposable)?.Dispose();
                _boxViews.Remove(view);
            }
        }
    }
}
