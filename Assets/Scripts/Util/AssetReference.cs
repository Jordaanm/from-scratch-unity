using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Util
{
    public class AssetReference<T> : MonoSingleton<AssetReference<T>> where T : Object
    {
        [System.Serializable]
        public class AssetEntry
        {
            public string key;
            [PreviewField]
            public T value;
        }

        [TableList]
        public List<AssetEntry> entries = new List<AssetEntry>();

        public T GetAsset(string key) {
            return entries.Find(x => x.key == key)?.value;
        }
    }
}