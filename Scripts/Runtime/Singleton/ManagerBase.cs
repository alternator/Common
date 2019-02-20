using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICKX {

	public abstract class ManagerBase<T> : MonoBehaviour where T : ManagerBase<T> {

		private static T s_instance = null;
		public static T Instance {
			get {
				if (s_instance == null) {
					s_instance = FindObjectOfType<T> ();
					if(s_instance == null) {
						s_instance = new GameObject (typeof (T).Name).AddComponent<T> ();
					}
					s_instance.Initialize ();
				}
				return s_instance;
			}
		}

		protected bool isInitialized = false;

		protected virtual void Initialize () {
			if (isInitialized) return;
			isInitialized = true;
			DontDestroyOnLoad (s_instance.gameObject);
		}

		protected void Awake () {
			if (s_instance == null) {
				s_instance = this as T;
			} else {
				if (s_instance != this) {
					Destroy (this);
				}
			}

			s_instance.Initialize ();
		}
	}
}