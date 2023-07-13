using UnityEditor;
using UnityEngine;

namespace UniOption.Editor {
    [CustomPropertyDrawer(typeof(Option<>), true)]
    public class OptionDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var contentProperty = property.FindPropertyRelative("_content");
            EditorGUI.PropertyField(position, contentProperty, new GUIContent(label));
            
        }
    }
}