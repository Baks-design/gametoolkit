using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameToolkit.Runtime.Utils.Helpers
{
    [Serializable]
    public class InterfaceReference<T>
        where T : class
    {
        [SerializeField]
        MonoBehaviour target;

        public T Value
        {
            get
            {
                if (target == null)
                    return null;
                return target as T;
            }
            set => target = value as MonoBehaviour;
        }

        public static implicit operator T(InterfaceReference<T> reference) => reference.Value;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InterfaceReference<>))]
    public class InterfaceReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetProp = property.FindPropertyRelative("target");

            EditorGUI.BeginProperty(position, label, property);

            // Get the interface type from the generic argument
            var interfaceType = fieldInfo.FieldType.GetGenericArguments()[0];

            EditorGUI.BeginChangeCheck();
            var newValue =
                EditorGUI.ObjectField(
                    position,
                    label,
                    targetProp.objectReferenceValue,
                    typeof(MonoBehaviour),
                    true
                ) as MonoBehaviour;

            if (EditorGUI.EndChangeCheck())
            {
                // Only assign if it implements the interface
                if (newValue == null || interfaceType.IsAssignableFrom(newValue.GetType()))
                    targetProp.objectReferenceValue = newValue;
                else
                    Logging.LogWarning($"Selected object doesn't implement {interfaceType.Name}");
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}
