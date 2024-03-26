using System;
using Atom.Client.Resources;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Atom.Client.Scenes
{
    [UsedImplicitly]
    public class LoadingScene : Scene
    {
        private LoadingSceneResources Resources { get; }

        private BackgroundTextureComponent _background;
        private DrawLabelComponent _loadingLabel;

        public LoadingScene([NotNull] LoadingSceneResources resources) : base(nameof(LoadingScene), resources)
        {
            Resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        protected override void Initialize()
        {
            _background = (BackgroundTextureComponent) AddView(new BackgroundTextureComponentDataModel
            {
                Texture = Resources.LoadingSceneBackgroundTexture
            });

            _loadingLabel = (DrawLabelComponent) AddView(new DrawLabelComponentDataModel
            {
                Font = Resources.Font,
                Color = Color.White,
                Position = new Vector2(10, 10),
                Text = "Loading..."
            });
        }

        protected override IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder)
        {
            return graphicsPipelineBuilder.Drawing2DPreset().Build();
        }
    }
}