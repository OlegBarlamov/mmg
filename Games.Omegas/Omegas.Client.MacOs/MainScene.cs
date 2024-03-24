using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;
using Template.MacOs.Models;

namespace Template.MacOs
{
    public class MainScene : Scene
    {
        private CharacterData CharacterData { get; } = new CharacterData();
        
        public MainScene(object model = null)
            : base("MainScene", model)
        {
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            
            AddController(CharacterData);
        }

        protected override IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder)
        {
            return graphicsPipelineBuilder.Drawing2DPreset(Color.CornflowerBlue, new BeginDrawConfig()).Build();
        }
    }
}