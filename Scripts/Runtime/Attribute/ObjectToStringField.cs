using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ICKX {

	public class ObjectToStringFieldAttribute : PropertyAttribute {

		public System.Type type;

		public ObjectToStringFieldAttribute (System.Type type = null) {
			this.type = type;
		}
	}
#if UNITY_EDITOR
	[CustomPropertyDrawer (typeof (ObjectToStringFieldAttribute))]
	public class ObjectToStringFieldDrawer : PropertyDrawer {

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			if (property.propertyType != SerializedPropertyType.String) {
				return;
			}
			var script = attribute as ObjectToStringFieldAttribute;

			EditorGUI.BeginProperty (position, label, property);

			string stringValue = EditorGUI.TextField (position, label, property.stringValue);

			if (position.Contains (Event.current.mousePosition)) {
				EventType eventType = Event.current.type;

				if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform) {
					//カーソルに+のアイコンを表示
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

					if (eventType == EventType.DragPerform) {
						var droped = DragAndDrop.objectReferences.FirstOrDefault ();

						if (droped != null && (script.type == null || droped.GetType () == script.type)) {
							stringValue = droped.name;
							DragAndDrop.AcceptDrag ();
						}
					}

					//イベントを使用済みにする
					Event.current.Use ();
				}
			}
			property.stringValue = stringValue;
			property.serializedObject.ApplyModifiedProperties ();

			EditorGUI.EndProperty ();
		}
	}
#endif
}