using UnityEngine;

namespace Roadmap
{
    public class TreeEdge<TDataType>
    {
        public TreeNode<TDataType> from;
        public TreeNode<TDataType> to;

        public TreeEdge(TreeNode<TDataType> from, TreeNode<TDataType> to)
        {
            this.from = from;
            this.to = to;
        }
    }
}