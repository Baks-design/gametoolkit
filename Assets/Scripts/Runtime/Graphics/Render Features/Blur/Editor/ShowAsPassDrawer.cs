using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.Blur
{
    [CustomPropertyDrawer(typeof(ShowAsPass))]
    public class ShowAsPassDrawer : PropertyDrawer
    {
        Type targetType;
        FieldInfo targetField;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var passAttribute = (ShowAsPass)attribute;

            var target = property.serializedObject.targetObject;
            var targetMaterialField = passAttribute.TargetMaterialField;

            targetType ??= target.GetType();
            targetField ??= targetType.GetField(
                targetMaterialField,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (targetField != null)
            {
                var fieldValue = targetField.GetValue(target);
                var material = fieldValue as Material;
                if (material != null)
                {
                    var selectablePasses = GetPassIndexStringEntries(material);
                    var choiceIndex = EditorGUI.Popup(
                        position,
                        label,
                        property.intValue,
                        selectablePasses.ToArray()
                    );

                    property.intValue = choiceIndex;
                }
                else
                    EditorGUI.HelpBox(
                        position,
                        $"Incorrect target field or Material not set.",
                        MessageType.Error
                    );
            }
            else
                EditorGUI.HelpBox(
                    position,
                    $"Field {targetMaterialField} not found on {targetType.Name}.",
                    MessageType.Error
                );

            EditorGUI.EndProperty();
        }

        List<GUIContent> GetPassIndexStringEntries(Material material)
        {
            var passIndexEntries = new List<GUIContent>();
            for (var i = 0; i < material.passCount; ++i)
            {
                var entry = $"{material.GetPassName(i)} ({i})";
                passIndexEntries.Add(new GUIContent(entry));
            }
            return passIndexEntries;
        }
    }
}
