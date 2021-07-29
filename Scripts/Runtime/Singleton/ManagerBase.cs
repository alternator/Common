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

                    if (!s_instance.isInitialized)
                    {
                        s_instance.isInitialized = true;
                        s_instance.Initialize();
                    }
                }
				return s_instance;
			}
		}

        private bool isInitialized = false;

		protected virtual void Initialize () {
		}

		protected void Awake () {
			if (s_instance == null) {
				s_instance = this as T;
			} else {
				if (s_instance != this) {
					Destroy (this);
				}
			}

            DontDestroyOnLoad(gameObject);

            if (!isInitialized)
            {
                isInitialized = true;
                Initialize();
            }
		}
	}
}