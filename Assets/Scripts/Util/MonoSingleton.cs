using UnityEngine;

namespace Util
{
	public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private const string MsgMultiInstance = "[Singleton] Multiple instances of a singleton gameObject '{0}'";

		private static T _instance;
		private static bool _showDebug = false;

		public static bool IS_EXITING = false;
		protected bool IsExiting => IS_EXITING;


		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void OnRuntimeStartSingleton() {
			IS_EXITING = false;
		}

		public static T Instance {
			get {

				bool isExiting = IS_EXITING;
#if UNITY_EDITOR
				isExiting = false;
#endif

				if (_instance == null && !isExiting) {
					_instance = (T)FindObjectOfType(typeof(T));

					if (FindObjectsOfType(typeof(T)).Length > 1) {
						DebugLogFormat(MsgMultiInstance, _instance.gameObject.name);

						return _instance;
					}

					if (_instance == null) {
						GameObject singleton = new GameObject();
						_instance = singleton.AddComponent<T>();
						singleton.name = string.Format("{0}(singleton)", typeof(T).ToString());

						MonoSingleton<T> component = _instance.GetComponent<MonoSingleton<T>>();
						component.OnCreate();

						if (component.ShouldNotDestroyOnLoad()) DontDestroyOnLoad(singleton);
						DebugLogFormat("[Singleton] Creating an instance of {0} with DontDestroyOnLoad", typeof(T));
					} else {
						DebugLogFormat("[Singleton] Using instance already created '{0}'", _instance.gameObject.name);
					}
				}

				return _instance;
			}
		}

		#region Virtual Methods
		protected virtual void OnCreate() {
			return;
		}

		protected void WakeUp() {
			return;
		}

		protected virtual bool ShouldNotDestroyOnLoad() {
			return true;
		}

		#endregion


		#region Private Messages

		private void OnApplicationQuit() {
			Debug.LogFormat("{0}::OnApplicationQuit", this.GetType().ToString());
			IS_EXITING = true;
		}

		private void OnDestroy() {
			_instance = null;
		}

		private static void DebugLogFormat(string content, params object[] parameters) {
			if (!_showDebug) return;
			Debug.LogFormat(content, parameters);
		}

		#endregion
	}
}
