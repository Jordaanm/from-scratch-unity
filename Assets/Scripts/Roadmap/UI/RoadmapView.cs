using System;
using System.Linq;
using System.Threading.Tasks;
using AssetReferences;
using UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Unlockable;
using Util;

namespace Roadmap.UI
{
    public class RoadmapView
    {
        private Roadmap roadmap;

        // UI References
        private const string VisualTreeAssetKey = "roadmap";
        private const string NodeVisualTreeAssetKey = "roadmap-node";
        private VisualElement root;
        private Label lblTitle;
        private VisualElement veNodes;
        private VisualElement veEdges;
        private VisualElement veRoadmap;
        
        public RoadmapView()
        {
            Init();
        }

        public RoadmapView(Roadmap roadmap)
        {
            this.roadmap = roadmap;
            Init();
            BindToRoadmap();
        }

        public RoadmapView(IUnlock unlock, int depth = 2)
        {
            roadmap = Roadmap.BuildFor(unlock, depth);
            Init();
            BindToRoadmap();
        }

        public VisualElement Root => root;

        private void Init()
        {
            root = VisualTreeAssetReference.Create(VisualTreeAssetKey);
            root.style.backgroundImage = new StyleBackground(UISpriteReferences.Instance.GetAsset("blueprint-bg"));
            lblTitle = root.Q<Label>("title");
            veRoadmap = root.Q("roadmap");
            veEdges = root.Q("edges");
            veNodes = root.Q("nodes");
        }

        public void SetRoadmap(Roadmap newRoadmap)
        {
            this.roadmap = newRoadmap;
            BindToRoadmap();
        }

        private async void BindToRoadmap()
        {
            //Set Title
            lblTitle.text = roadmap.name;

            //Clear NodeViews
            veNodes.Clear();
            //Clear ConnectionViews
            veEdges.Clear();

            //Generate NodeViews
            foreach (var node in roadmap.Nodes)
            {
                GenerateNodeView(node);
            }
            //Generate ConnectionViews
            
            await Task.Delay(100); //Wait .1s so the Node views have resolved dimensions/positions//
            foreach (var edge in roadmap.Edges)
            {
                GenerateEdge(edge);
            }
        }

        private void GenerateEdge(TreeEdge<IUnlock> edge)
        {
            var edgeVE = new VisualElement();
            edgeVE.AddToClassList("edge");

            Vector2 avgPos = (edge.from.position + edge.to.position) / 2;
            Vector2 delta = (edge.to.position - edge.from.position);
            float height = Mathf.Min(8, Vector2.Distance(edge.@from.position, edge.to.position));
            float angle = Vector2.SignedAngle(Vector2.right, delta);

            edgeVE.style.width = new Length(height, LengthUnit.Percent);
            edgeVE.style.left = new Length(avgPos.x, LengthUnit.Percent);
            edgeVE.style.top = new Length(avgPos.y, LengthUnit.Percent);
            edgeVE.style.rotate = new StyleRotate(new Rotate(new Angle(angle, AngleUnit.Degree)));
            edgeVE.style.backgroundImage = new StyleBackground(UISpriteReferences.Instance.GetAsset("roadmap-arrow-straight"));
            veEdges.Add(edgeVE);
        }

        private Texture2D DrawSplineToTexture(float width, float height, bool ascending = true, int steps = 256)
        {
            int w = Mathf.CeilToInt(width);
            int h = Mathf.CeilToInt(height);
            var tex = new Texture2D(w, h);

            //Map to Positions
            Vector2 fromPos = new Vector2(ascending ? 0: w, 0);
            Vector2 toPos = new Vector2(ascending ? width : 0, height);

            Vector2 midA = new Vector2(fromPos.x, (fromPos.y + toPos.y) / 2);
            Vector2 midB = new Vector2(toPos.x, (fromPos.y + toPos.y) / 2);

            var iter = 1.0f / steps;

            var points = Enumerable.Range(0, steps)
                .Select(x => x * iter)
                .Select(x => Spline.GetPoint(fromPos, midA, midB, toPos, x))
                .ToArray();

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }

            foreach (var point in points) {
                tex.SetPixel((int)point.x, (int)point.y, Color.yellow);
            }

            tex.Apply();

            return tex;
        }
        
        private void GenerateNodeView(RoadmapTreeNode node)
        {
            var nodeRoot = VisualTreeAssetReference.Create(NodeVisualTreeAssetKey);
            nodeRoot.userData = node;
            var backgroundSpriteKey = node.isCentral ? "roadmap-rect" : "roadmap-capsule";
            nodeRoot.style.backgroundImage =
                new StyleBackground(UISpriteReferences.Instance.GetAsset(backgroundSpriteKey));
            nodeRoot.Q<Label>("label").text = node.data.ID;
            nodeRoot.style.left = new Length(node.position.x, LengthUnit.Percent);
            nodeRoot.style.top = new Length(node.position.y, LengthUnit.Percent);

            //TODO: Add Icon
            
            string significance = node.isCentral ? "central" : node.data.Significance.ToString().ToLower();
            nodeRoot.AddToClassList($"node--{significance}");

            veNodes.Add(nodeRoot);
        }
    }
}