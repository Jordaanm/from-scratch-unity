using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unlockable;
using Util;

namespace Roadmap
{
    public class Roadmap: FeatureTree<RoadmapTreeNode, IUnlock>
    {
        private const float NodeDistance = 20f;
        
        private List<RoadmapTreeNode> nodes = new();
        private List<TreeEdge<IUnlock>> edges = new();

        private string id;
        public string name;

        public List<RoadmapTreeNode> Nodes => nodes;
        public List<TreeEdge<IUnlock>> Edges => edges;

        public RoadmapTreeNode AddNode(IUnlock unlock, RoadmapTreeNode parent)
        {
            var node = new RoadmapTreeNode(unlock);
            nodes.Add(node);
            if (parent != null)
            {
                edges.Add(new TreeEdge<IUnlock>(node, parent));
            }
            return node;
        }

        public string GetID()
        {
            return id;
        }

        public static Roadmap BuildFor(IUnlock unlockable, int depth = 2)
        {
            Roadmap roadmap = new Roadmap();
            roadmap.SetName(unlockable.ID); //TODO: Replace with relevant name field instead.
            AddNodesToRoadmap(roadmap, null, unlockable, 2);

            roadmap.LayoutNodes(unlockable);

            return roadmap;
        }

        private void LayoutNodes(IUnlock keystone)
        {
            RoadmapTreeNode node = GetNodeForUnlock(keystone);
            node.position = new Vector2(50, 50);
            node.significance = NodeSignificance.Key;

            LayoutChildNodes(node);
        }

        private void LayoutChildNodes(RoadmapTreeNode node, float angle = 0, float spread = 6.28f)
        {
            var childNodes = GetChildNodes(node);
            var nodeCount = childNodes.Count;

            var childSpread = nodeCount == 0 ? 0: spread / nodeCount;
            var startingAngle = angle - (childSpread * 0.5f);
            
            for (int i = 0; i < nodeCount; ++i)
            {
                var childNode = childNodes[i];
                var childAngle = startingAngle + childSpread * i;
                childNode.position = new Vector2(node.position.x, node.position.y);
                childNode.position.x += Mathf.Sin(childAngle) * NodeDistance;
                childNode.position.y += Mathf.Cos(childAngle) * NodeDistance;
                LayoutChildNodes(childNode, childAngle, childSpread);
            }
        }

        private List<RoadmapTreeNode> GetChildNodes(RoadmapTreeNode node)
        {
            return edges
                .FindAll(x => x.to == node)
                .Select(x => x.from as RoadmapTreeNode)
                .ToList();
        }


        private RoadmapTreeNode GetNodeForUnlock(IUnlock keystone)
        {
            return nodes.Find(x => x.data == keystone);
        }

        private void SetName(string name)
        {
            this.name = name;
        }

        public static void AddNodesToRoadmap(Roadmap roadmap, RoadmapTreeNode parent, IUnlock unlockable, int depth)
        {
            var node = roadmap.AddNode(unlockable, parent);

            if (depth == 0)
            {
                return;
            }

            if (unlockable.Prerequisites != null)
            {
                var prereqUnlocks = unlockable.Prerequisites
                    .Select(x => IUnlock.FromPrereq(x));
            
                foreach (var child in prereqUnlocks)
                {
                    AddNodesToRoadmap(roadmap, node, child, depth - 1);
                }
            }
        }
    }
}