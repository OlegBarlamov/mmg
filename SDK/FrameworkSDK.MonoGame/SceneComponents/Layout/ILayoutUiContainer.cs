namespace FrameworkSDK.MonoGame.SceneComponents.Layout
{
    public interface ILayoutUiContainer : ILayoutUiNode
    {
        ILayoutUiContainer AddChild(ILayoutUiNode node);
        ILayoutUiContainer RemoveChild(ILayoutUiNode node);

        void Invalidate(ILayoutUiNode child);
        bool CheckVisible();
    }
}