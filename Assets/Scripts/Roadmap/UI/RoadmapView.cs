using System;
using System.Linq;
using System.Threading.Tasks;
using AssetReferences;
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
            // await Task.Delay(1000); //Wait 1s so the Node views have resolved dimensions/positions//
            // GenerateEdge(roadmap.Edges[0]);
        }

        private void GenerateEdge(TreeEdge<IUnlock> edge)
        {
            var edgeVE = new VisualElement();
            edgeVE.AddToClassList("edge");

            var fromVE = veNodes.Children().ToList().Find(x => x.userData == edge.from);
            var toVE = veNodes.Children().ToList().Find(x => x.userData == edge.to);

            
            //Find rectangle between their centers
            var vEdgeA = fromVE.style.top;
            var vEdgeB = toVE.style.top;
            var hEdgeA = fromVE.style.left;
            var hEdgeB = toVE.style.left;

            var top = Mathf.Min(vEdgeA.value.value, vEdgeB.value.value);
            var left = Mathf.Min(hEdgeA.value.value, hEdgeB.value.value);
            var height = Mathf.Max(1, Mathf.Abs(fromVE.resolvedStyle.top - toVE.resolvedStyle.top));
            var width = Mathf.Max(1, Mathf.Abs(fromVE.resolvedStyle.left - toVE.resolvedStyle.left));

            bool isAscending = (vEdgeA.value.value > vEdgeB.value.value) != (hEdgeA.value.value > hEdgeB.value.value);

            edgeVE.style.left = new Length(left, LengthUnit.Percent);
            edgeVE.style.width = new Length(width, LengthUnit.Pixel);
            edgeVE.style.top = new Length(top, LengthUnit.Percent);
            edgeVE.style.height = new Length(height, LengthUnit.Pixel);
            
            Texture2D tex = DrawSplineToTexture(width, height, isAscending);
            edgeVE.style.backgroundImage = new StyleBackground(tex);
            veEdges.Add(edgeVE);
        }

        private Texture2D DrawSplineToTexture(float width, float height, bool ascending = true, int steps = 256)
        {
            int w = Mathf.CeilToInt(width);
            int h = Mathf.CeilToInt(height);
            var tex = new Texture2D(w, h);
            tex.filterMode = FilterMode.Bilinear;

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
            nodeRoot.Q<Label>("label").text = node.data.ID;
            nodeRoot.style.left = new Length(node.position.x, LengthUnit.Percent);
            nodeRoot.style.top = new Length(node.position.y, LengthUnit.Percent);
            
            veNodes.Add(nodeRoot);
        }
    }
}