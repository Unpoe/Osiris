using System;
using UnityEngine;
using UnityEditor;

namespace Osiris
{
    [CustomPropertyDrawer(typeof(EnumLabelArrayAttribute))]
    public class EnumLabelArrayAttributeEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (property.isExpanded) {
                float total = 40;
                int depth = property.depth;
                if (property.hasChildren) {
                    property.Next(true);
                    total += EditorGUI.GetPropertyHeight(property);
                    while (property.Next(false) && property.depth > depth) {
                        total += EditorGUI.GetPropertyHeight(property);
                    }
                }
                return total;
            } else {
                return 20f;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EnumLabelArrayAttribute att = attribute as EnumLabelArrayAttribute;
            // PropertyPath returns something like component_hp_max.Array.data[4] so get the index from there
            int index = Convert.ToInt32(property.propertyPath.Substring(property.propertyPath.LastIndexOf("[")).Replace("[", "").Replace("]", ""));
            // Change the label
            label.text = att.labels[index];
            // Draw field
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}