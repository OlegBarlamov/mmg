
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Services;
using FrameworkSDK.MonoGame.Services;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public interface IGraphicsPipelineBuilder
    {
        IRenderTargetsFactoryService RenderTargetsFactoryService { get; }

        IGraphicsPipelineBuilder AddAction(IGraphicsPipelineAction action);

        IGraphicsPipeline Build();

        VertexBuffer CreateVertexBugger(VertexDeclaration vertexDeclaration, int vertexCount);

        IndexBuffer CreateIndexBuffer(int indicesCount);
    }
}