using System;
using FromScratch.Crafting;
using Roadmap.UI;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Util;

namespace Roadmap.Editor
{
    public class RoadmapEditorWindow: OdinMenuEditorWindow
    {
        private VisualElement root;
        private RoadmapView roadmapView;
        private static readonly string stylesheetPath = "Assets/UI/Roadmap/roadmap.uss";
        private static readonly string commonStylesheetPath = "Assets/UI/common.uss";
        private static readonly string utilStylesheetPath = "Assets/UI/util.uss";



        [MenuItem("From Scratch/Roadmap")]
        public static void ShowSkillTreeWindow() {
            RoadmapEditorWindow wnd = GetWindow<RoadmapEditorWindow>();
            wnd.titleContent = new GUIContent("Roadmap Editor");
        }

        

        public void OnEnable()
        {
            Debug.Log("RoadmapEditorWindow::OnEnable");
            if (root == null)
            {
                Recipe boatHull = GameDatabases.Instance.recipes.GetDatabase()["boat_hull"];
                Roadmap roadmap = Roadmap.BuildFor(boatHull);
                roadmapView = new RoadmapView(roadmap);
                root = roadmapView.Root;
                
                rootVisualElement.Add(root);
                rootVisualElement.AddToClassList("full");
                rootVisualElement.AddToClassList("debugging");
                var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(stylesheetPath);
                rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(commonStylesheetPath));
                rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(utilStylesheetPath));
                rootVisualElement.styleSheets.Add(styleSheet);
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.AddAllAssetsAtPath("Roadmaps", "Assets/Data/Roadmaps", typeof(Roadmap), false, false);
            return tree;
        }
    }
}