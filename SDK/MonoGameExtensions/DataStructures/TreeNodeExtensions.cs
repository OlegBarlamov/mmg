using System;
using System.Collections.Generic;

namespace MonoGameExtensions.DataStructures
{
    public static class TreeNodeExtensions
    {
        public static TParent FindAncestor<T, TParent>(this ITreeNode<T, TParent> node, Func<TParent, bool> predicate) 
            where T : ITreeNode<T, TParent>
            where TParent : ITreeNode<T, TParent>
        {
            var x = node.Parent;
            while (x != null)
            {
                if (predicate(x))
                    return x;
                
                x = x.Parent;
            }

            return default;
        }

        public static void TraverseChildren<T, TParent>(this ITreeNode<T, TParent> node, Action<T> action)
            where T : ITreeNode<T, TParent>
            where TParent : ITreeNode<T, TParent>
        {
            var queue = new Queue<T>(node.Children);
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                action(item);

                foreach (var itemChild in item.Children)
                    queue.Enqueue(itemChild);
            }
        }
    }
}