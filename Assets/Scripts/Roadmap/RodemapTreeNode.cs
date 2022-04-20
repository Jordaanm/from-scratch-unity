using System;
using UnityEngine;
using Unlockable;
using Util;

namespace Roadmap
{
    public class RoadmapTreeNode: TreeNode<IUnlock>
    {
        public RoadmapTreeNode(IUnlock data)
        {
            this.data = data;
        }

        public override string ID => data.ID;
    }
}