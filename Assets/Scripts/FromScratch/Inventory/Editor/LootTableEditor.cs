using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FromScratch.Inventory
{
    [CustomEditor(typeof(LootTable))]
    public class LootTableEditor : UnityEditor.Editor
    {
        const string treeAssetPath = "Assets/UI/Inventory/Editor/LootTableEditor.uxml";
        const string styleAssetPath = "Assets/UI/Inventory/Editor/LootTable.uss";
        static VisualTreeAsset treeAsset;
        static StyleSheet styleSheet;

        VisualElement root;
        VisualElement summaryGraphContainer;
        VisualElement summaryLegendContainer;

        LootTable lootTable;

        public static readonly Color[] GRAPH_COLORS = new Color[]
        {
            Color.cyan,
            Color.yellow,
            Color.green,
            Color.red,
            Color.blue,
            Color.magenta
        };

        private VisualElement GetTemplate()
        {
            if (treeAsset == null)
            {
                treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(treeAssetPath);
            }

            if (styleSheet == null)
            {
                styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleAssetPath);
            }

            VisualElement ve = treeAsset.CloneTree().Children().First();
            ve.styleSheets.Add(styleSheet);
            return ve;
        }

        public override VisualElement CreateInspectorGUI()
        {
            root = GetTemplate();
            root.Bind(serializedObject);

            lootTable = (LootTable)target;

            root.Q<PropertyField>("prop__loot").BindProperty(serializedObject.FindProperty("loot"));
            root.Q<PropertyField>("prop__loot").label = "Loot";

            root.Q<Button>("refresh-summary").clicked += () =>
            {
                UpdateSummary();
                UpdateSummaryLegend();
            };
            summaryGraphContainer = root.Q("graph-container");
            summaryLegendContainer = root.Q("legend-container");


            UpdateSummary();
            UpdateSummaryLegend();

            return root;
        }

        private void UpdateSummary()
        {
            summaryGraphContainer.Clear();
            summaryGraphContainer.style.height = new StyleLength(50);
            summaryGraphContainer.style.flexDirection = FlexDirection.Row;

            int index = 0;

            if (lootTable.noDropWeight > 0)
            {
                VisualElement el = new VisualElement()
                {
                    style =
                    {
                        flexGrow = lootTable.noDropWeight,
                        backgroundColor = Color.gray
                    }
                };
                summaryGraphContainer.Add(el);
            }

            foreach (var lootItem in lootTable.loot)
            {
                VisualElement el = new VisualElement()
                {
                    style =
                    {
                        flexGrow = lootItem.weight,
                        backgroundColor = GetColor(index)
                    }
                };

                summaryGraphContainer.Add(el);
                ++index;
            }

        }

        private Color GetColor(int index)
        {
            return GRAPH_COLORS[index % GRAPH_COLORS.Length];
        }

        private void UpdateSummaryLegend()
        {
            summaryLegendContainer.Clear();
            int index = 0;

            if (lootTable.noDropWeight > 0)
            {
                VisualElement el = CreateSummaryLegendItem("No Drop", Color.gray);
                summaryLegendContainer.Add(el);
            }

            foreach (var lootItem in lootTable.loot)
            {
                string itemName = lootItem?.itemData?.itemName == null ? "No Item" : lootItem?.itemData?.itemName;
                string label = string.Format("{0} x{1}", itemName, lootItem.amount);
                VisualElement el = CreateSummaryLegendItem(label, GetColor(index));
                summaryLegendContainer.Add(el);
                ++index;
            }
        }

        private VisualElement CreateSummaryLegendItem(string label, Color color)
        {
            VisualElement el = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    marginBottom = 5
                }
            };

            VisualElement box = new VisualElement()
            {
                style =
                {
                    height = 10,
                    width = 10,
                    backgroundColor = color,
                    marginRight = 10
                }
            };
            el.Add(box);
            el.Add(new Label(label));

            return el;
        }
    }
}