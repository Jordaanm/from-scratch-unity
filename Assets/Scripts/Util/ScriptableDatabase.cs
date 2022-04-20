using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Util
{
    public class ScriptableDatabase<T> : SerializedScriptableObject
    where T: ScriptableData
    {
        public List<T> entries = new List<T>();

        protected Dictionary<string, T> database;

        public Dictionary<string, T> GetDatabase()
        {
            if (database == null)
            {
                database = BuildDatabase();
            }

            return database;
        }

        protected Dictionary<string, T> BuildDatabase()
        {
            database = new Dictionary<string, T>();

            entries.ForEach(entry =>
            {
                if (entry == null) return;
                var id = entry.GetID();
                if (id != null || id == "")
                {
                    try
                    {
                        database.Add(entry.GetID(), entry);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("woooo");
                    }
                }
                else
                {
                    Debug.LogErrorFormat("No ID present for Entry: {0}", entry.name);
                }
            });
            
            return database;
        }
        
        #if UNITY_EDITOR
        [Button]
        public void SyncWithAssets()
        {
            List<T> syncedEntries = new List<T>();
            string assetFolderPath = GetFolderPath();
            
            IEnumerable<string> paths = AssetDatabase.GetAllAssetPaths()
                .Where<string>(x => x.StartsWith(assetFolderPath, StringComparison.InvariantCultureIgnoreCase));
            foreach (string path in paths)
            {
                T entry = AssetDatabase.LoadAssetAtPath<T>(path);
                if (entry != null)
                {
                    syncedEntries.Add(entry);        
                }
            }
            
            entries.Clear();
            entries.AddRange(syncedEntries);
            database = BuildDatabase();
        }

        protected string GetFolderPath()
        {
            var method = typeof(T).GetMethod("GetAssetPath", BindingFlags.Static);
            string assetFolderPath = method == null ? "Assets/Data" : method.Invoke(null, null).ToString();
            
            assetFolderPath = (assetFolderPath ?? "").TrimEnd('/') + "/";      
            
            string lower = assetFolderPath.ToLower();
            if (!lower.StartsWith("assets/") && !lower.StartsWith("packages/"))
                assetFolderPath = "Assets/" + assetFolderPath;
            
            assetFolderPath = assetFolderPath.TrimEnd('/') + "/";
            return assetFolderPath;
        }
        
        #endif
    }
}