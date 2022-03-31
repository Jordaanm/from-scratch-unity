using FromScratch.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FromScratch.Gathering
{
    [CreateAssetMenu(menuName = "From Scratch/Gathering/ResourceNodeInfo", fileName = "MyResourceNodeInfo")]
    public class ResourceNodeInfo: ScriptableObject
    {
        public string nodeName = "My Resource Node";

        [AssetSelector(Paths = "Assets/Data/Loot Tables/"), InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        public LootTable lootTable;
        public bool isRepeatable = true;
        public int initialStock = 99;

        [PreviewField, InlineEditor(InlineEditorModes.SmallPreview)]
        [BoxGroup("Animation")]
        public AnimationClip animation;
        [BoxGroup("Animation")]
        public float animationDuration = -1f;
    }
}