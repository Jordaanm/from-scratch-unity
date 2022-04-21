using System;
using UnityEngine;
using Unlockable;
using Util;

namespace Roadmap
{
    public class RoadmapTreeNode: TreeNode<IUnlock>
    {
        public bool isCentral; 
        public RoadmapTreeNode(IUnlock data, bool isCentral = false)
        {
            this.isCentral = isCentral;
            this.data = data;
        }

        public override string ID => data.ID;
    }
}