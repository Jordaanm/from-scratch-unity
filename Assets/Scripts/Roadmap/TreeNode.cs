using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Roadmap
{
    public enum NodeSignificance
    {
        Minor,
        Major,
        Key
    }
    
    [Serializable]
    public abstract class TreeNode<T>
    {
        public Vector2 position;
        public abstract string ID { get; }
        public NodeSignificance significance;
        [InlineEditor(InlineEditorModes.FullEditor)]
        public T data;

        public TreeNode()
        {
            position = new Vector2(0, 0);
        }
    }
}