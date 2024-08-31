using System.Collections.Generic;

namespace MonoGameExtensions.DataStructures
{
    public interface ITreeNode<out TNode, TParent> where TNode : ITreeNode<TNode, TParent> where TParent : ITreeNode<TNode, TParent>
    {
        TParent Parent { get; set; }
        
        IReadOnlyCollection<TNode> Children { get; }
    }
}