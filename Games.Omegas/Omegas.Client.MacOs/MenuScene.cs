using System;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents.Layout.Attributes;
using FrameworkSDK.MonoGame.SceneComponents.Layout.Elements;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Omegas.Client.MacOs
{
    public class MenuScene : Scene
    {
        public MenuResourcePackage ResourcePackage { get; }

        public MenuScene([NotNull] MenuResourcePackage resourcePackage) : base("MenuScene")
        {
            ResourcePackage = resourcePackage ?? throw new ArgumentNullException(nameof(resourcePackage));
        }

        protected override void Initialize()
        {
            UI.AddChild(
                new StackPanelElementViewModel
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 200,
                    Height = 500,
                }.AddChild(new TextBoxElementViewModel
                    {
                        Text = "Hello 1",
                        Font = ResourcePackage.MenuFont,
                        Color = Color.White,
                        Width = 100,
                    }
                ).AddChild(new TextBoxElementViewModel
                    {
                        Text = "Hello 2",
                        Font = ResourcePackage.MenuFont,
                        Color = Color.White,
                        Width = 100,
                    }
                )
            );
        }

        protected override IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder)
        {
            return graphicsPipelineBuilder.Drawing2DPreset(Color.Black).Build();
        }
    }
}