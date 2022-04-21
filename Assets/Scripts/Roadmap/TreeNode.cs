using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Roadmap
{
    [Serializable]
    public abstract class TreeNode<T>
    {
        public Vector2 position;
        public abstract string ID { get; }
        [InlineEditor(InlineEditorModes.FullEditor)]
        public T data;

        public TreeNode()
        {
            position = new Vector2(0, 0);
        }
    }
}