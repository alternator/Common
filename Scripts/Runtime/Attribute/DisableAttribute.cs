using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace ICKX {

	public class DisableAttribute : PropertyAttribute {

	}

#if UNITY_EDITOR
	[CustomPropertyDrawer (typeof (DisableAttribute))]
	public class DisableDrawer : PropertyDrawer {

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginDisabledGroup (true);
			EditorGUI.PropertyField (position, property, label, true);
			EditorGUI.EndDisabledGroup ();
		}
	}
#endif
}