using System;
using System.Linq;
using FromScratch.Equipment;
using FromScratch.Inventory;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace FromScratch.Editor
{
    public class DataManager : OdinMenuEditorWindow
    {
        private static Type[] typesToDisplay = TypeCache.GetTypesWithAttribute<ManageableDataAttribute>()
            .OrderBy(m => m.Name)
            .ToArray();

        private Type selectedType;

        [MenuItem("From Scratch/Data Manager")]
        private static void OpenEditor() => GetWindow<DataManager>();

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            foreach (var type in typesToDisplay)
            {
                tree.AddAllAssetsAtPath(type.Name, "Assets/", type, true, true);
            }

            return tree;
        }
        
        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Item")))
                {
                    ScriptableObjectCreator.ShowDialog<ItemData>("Assets/Data/Items", obj =>
                    {
                        obj.itemName = obj.name;
                        base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    });
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Equipment")))
                {
                    ScriptableObjectCreator.ShowDialog<EquipmentData>("Assets/Data/Equipment", obj =>
                    {
                        obj.equipmentName = obj.name;
                        base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    });
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}