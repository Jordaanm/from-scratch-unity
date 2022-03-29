using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class AssetReference<T> : MonoSingleton<AssetReference<T>> where T : Object
    {
        [System.Serializable]
        public class AssetEntry
        {
            public string key;
            public T value;
        }

        public List<AssetEntry> entries = new List<AssetEntry>();

        public T GetAsset(string key) {
            return entries.Find(x => x.key == key)?.value;
        }
    }
}