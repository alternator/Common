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
					if (s_instance != null) {
                        if (!s_instance.isInitialized)
                        {
                            s_instance.isInitialized = true;
                            s_instance.Initialize();
                        }
                    }
                }
				return s_instance;
			}
		}

		protected virtual bool isDontDestroyOnLoad { get { return false; } }

		private bool isInitialized = false;

		protected virtual void Initialize () {
		}

		protected void Awake () {
            if(s_instance != null) {
                if(s_instance != this) {
					Debug.LogError($"{typeof(T).FullName}重複したSingletonがあります", s_instance);
                    Destroy (s_instance);
				}
			}
			s_instance = this as T;

			if (s_instance.isDontDestroyOnLoad) {
				DontDestroyOnLoad (s_instance.gameObject);
			}

            if (!isInitialized)
            {
                isInitialized = true;
                Initialize();
            }
        }
    }
}
