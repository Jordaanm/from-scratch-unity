using System.Linq;
using UnityEngine.UIElements;

namespace AssetReferences
{
    public class VisualTreeAssetReference : Util.AssetReference<VisualTreeAsset>
    {
        public static VisualElement Create(string key)
        {
            VisualTreeAsset visualTreeAsset = VisualTreeAssetReference.Instance.GetAsset(key);
            return Create(visualTreeAsset);
        }

        public static VisualElement Create(VisualTreeAsset visualTreeAsset)
        {
            if (visualTreeAsset == null)
            {
                return null;
            }
            return visualTreeAsset.CloneTree().Children().First();
        }
    }
}
