using UnityEngine;

namespace Util
{
    public class CoroutinesManager : MonoSingleton<CoroutinesManager>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnRuntimeStartSingleton() {
            IS_EXITING = false;
        }
    }
}