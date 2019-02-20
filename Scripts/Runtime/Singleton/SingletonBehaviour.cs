using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICKX {
	public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T> {

		private static T s_instance = null;
		public static T Instance {
			get {
				if (s_instance == null) {
					s_instance = FindObjectOfType<T> ();
					if (s_instance != null && !Instance.isInitialized) {
						s_instance.Initialize ();
					}
				}
				return s_instance;
			}
		}

		protected virtual bool isDontDestroyOnLoad { get { return false; } }

		protected bool isInitialized = false;

		protected virtual void Initialize () {
			if (isInitialized) return;
			isInitialized = true;
		}

		protected void Awake () {
            if(s_instance == null) {
                s_instance = this as T;
            }else {
                if(s_instance != this) {
                    Destroy (this);
                }
            }

			if (s_instance.isDontDestroyOnLoad) {
				DontDestroyOnLoad (s_instance.gameObject);
			}
			if(!isInitialized) {
				Initialize ();
			}
		}
	}
}
