using UnityEngine;
using System.Collections;

namespace ICKX {
	[ExecuteInEditMode]
	public class CoroutineManager : MonoBehaviour {

		static private CoroutineManager instance;
		static public CoroutineManager Instance {
			get {
				if (instance == null) {
					instance = new GameObject ("CoroutineManager").AddComponent<CoroutineManager> ();
					DontDestroyOnLoad (instance.gameObject);
					instance.hideFlags = HideFlags.DontSave;
				}
				return instance;
			}
		}

		static public Coroutine Start (IEnumerator routine) {
			return Instance.StartCoroutine (routine);
		}
		static public void Stop (IEnumerator routine) {
			instance.StopCoroutine (routine);
		}

		void Update () {
			if (!Application.isPlaying) {
				StopAllCoroutines ();
				DestroyImmediate (gameObject);
			}
		}
	}
}