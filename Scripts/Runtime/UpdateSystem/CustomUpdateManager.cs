//#define CHECK_COMPONENT_NULL
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICKX {

	public class CustomUpdateManager : MonoBehaviour {

		#region Singleton
		private static CustomUpdateManager _Instance;

		public static CustomUpdateManager Instance {
			get {
				if (_Instance == null) {
					_Instance = FindObjectOfType<CustomUpdateManager> ();
				}
				return _Instance;
			}
		}

		[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeOnLoad () {
			_Instance = new GameObject ("CustomUpdateManager").AddComponent<CustomUpdateManager> ();
			DontDestroyOnLoad (_Instance.gameObject);

			_Instance.extUpdateComponentList = new LinkedList<MonoBehaviour> ();
			_Instance.updatePhaseLastComponents = new LinkedListNode<MonoBehaviour>[256];
		}
		#endregion

		private LinkedList<MonoBehaviour> extUpdateComponentList;
		private LinkedListNode<MonoBehaviour>[] updatePhaseLastComponents;

		void Update () {
			if (extUpdateComponentList == null) return;

			float deltaTime = Time.deltaTime;
			var node = extUpdateComponentList.First;

			while (node != null) {
#if CHECK_COMPONENT_NULL
			if (node.Value != null) {
#endif
				var extInit = node.Value as IExtInitializable;
				if (extInit != null) {
					if (extInit.Initialize ()) {

						var updatable = node.Value as IExtUpdatable;
						if (updatable != null && updatable.IsEnableUpdate ()) {
							updatable.ExtUpdate (deltaTime);
						}
					}
				} else {
					var updatable = node.Value as IExtUpdatable;
					if (updatable.IsEnableUpdate () && updatable.IsInitializeComplete ()) {
						updatable.ExtUpdate (deltaTime);
					}
				}
#if CHECK_COMPONENT_NULL
			}
#endif
				node = node.Next;
			}

		}

		void FixedUpdate () {
			if (extUpdateComponentList == null) return;

			float deltaTime = Time.deltaTime;
			var node = extUpdateComponentList.First;

			while (node != null) {
#if CHECK_COMPONENT_NULL
			if (node.Value != null) {
#endif

				var updatable = node.Value as IExtFixedUpdatable;
				if (updatable != null && updatable.IsEnableUpdate () && updatable.IsInitializeComplete ()) {
					updatable.ExtFixedUpdate (deltaTime);
				}

#if CHECK_COMPONENT_NULL
			}
#endif
				node = node.Next;
			}
		}

		void LateUpdate () {
			if (extUpdateComponentList == null) return;

			float deltaTime = Time.deltaTime;
			var node = extUpdateComponentList.First;

			while (node != null) {
#if CHECK_COMPONENT_NULL
			if (node.Value != null) {
#endif

				var updatable = node.Value as IExtLateUpdatable;
				if (updatable != null && updatable.IsEnableUpdate () && updatable.IsInitializeComplete ()) {
					updatable.ExtLateUpdate (deltaTime);
				}

#if CHECK_COMPONENT_NULL
			}
#endif
				node = node.Next;
			}
		}

		public static void RegisterComponent (MonoBehaviour component, sbyte phase = 0) {
			if (_Instance == null) {
				Debug.LogException (new System.Exception (
					"CustomUpdateManagerより先に生成されるオブジェクトが存在してはいけません"), component);
				return;
			}

			LinkedListNode<MonoBehaviour> current = null;

			if (Instance.extUpdateComponentList.Count == 0) {
				//1要素も追加されていない場合
				current = Instance.extUpdateComponentList.AddLast (component);
			} else {
				var phaseLast = Instance.updatePhaseLastComponents[phase + 128];
				sbyte prevPhase = phase;
				while (phaseLast == null) {
					//指定のフェイズで登録しているコンポーネントがなければ、その前のフェイズのコンポーネントの後に入れたい
					prevPhase--;
					if (prevPhase < -128) {
						//前のフェイズになにもなければリストの最初に挿入
						current = Instance.extUpdateComponentList.AddFirst (component);
						break;
					} else {
						phaseLast = Instance.updatePhaseLastComponents[prevPhase + 128];
					}
				}
				if (current == null) {
					current = Instance.extUpdateComponentList.AddAfter (phaseLast, component);
				}
			}

			Instance.updatePhaseLastComponents[phase + 128] = current;
		}

		public static void RemoveComponent (MonoBehaviour component) {
			if (_Instance == null) {
				return;
			}

			Instance.extUpdateComponentList.Remove (component);
		}

		public static void PurgeAllComponent () {
#if UNITY_EDITOR
			Debug.Log ("登録解除できていないComponent数 : " + Instance.extUpdateComponentList.Count);
#endif
			Instance.extUpdateComponentList.Clear ();
		}
	}
	/// <summary>
	/// 数多く生成されるシーン中のオブジェクトのみ利用してください。
	/// シーンを跨いで利用されうるオブジェクト (Manager / UIなど)には利用しないでください
	/// </summary>
	public interface IExtInitializable {

		bool IsInitializeComplete ();

		bool Initialize ();
	}

	public interface IExtUpdatable : IExtInitializable {

		bool IsEnableUpdate ();

		void ExtUpdate (float deltaTime);
	}

	public interface IExtFixedUpdatable : IExtInitializable {

		bool IsEnableUpdate ();

		void ExtFixedUpdate (float deltaTime);
	}


	public interface IExtLateUpdatable : IExtInitializable {

		bool IsEnableUpdate ();

		void ExtLateUpdate (float deltaTime);
	}
}