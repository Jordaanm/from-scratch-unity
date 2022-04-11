using System;
using UnityEngine;

namespace Util
{
    public class GameObjectUtils
    {
        public static void TraverseHierarchy(GameObject gameObject, Action<GameObject> callback)
        {
            callback.Invoke(gameObject);
            var childCount = gameObject.transform.childCount;
            for (int x = childCount - 1; x >= 0; --x)
            {
                TraverseHierarchy(gameObject.transform.GetChild(x).gameObject, callback);
            }
        }
    }
}