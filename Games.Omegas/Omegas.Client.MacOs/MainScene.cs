using System;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Omegas.Client.MacOs.Models;

namespace Omegas.Client.MacOs
{
    public class MainScene : Scene
    {
        public IInputService InputService { get; }
        private CharacterData CharacterData { get; } = new CharacterData();
        
        public MainScene([NotNull] IInputService inputService, object model = null)
            : base("MainScene", model)
        {
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
        }

        protected override void Initialize()
        {
            AddController(CharacterData);
        }

        protected override IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder)
        {
            return graphicsPipelineBuilder.Drawing2DPreset(Color.CornflowerBlue, new BeginDrawConfig()).Build();
        }
    }
}