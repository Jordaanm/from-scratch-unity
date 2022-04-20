using System;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Roadmap
{
    public interface FeatureTree<TNodeType, TDataType>
        where TNodeType: TreeNode<TDataType>
    {
        public List<TNodeType> Nodes { get; }
        public string GetID();
    }
}