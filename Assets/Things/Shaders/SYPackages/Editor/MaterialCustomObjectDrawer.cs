using CustomShaderEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace CustomShaderEditor
{
    public class MaterialCustomObjectDrawer : MaterialPropertyDrawer
    {
        private readonly string groupName;
        private readonly string enableKeyword;
        

        public MaterialCustomObjectDrawer(string groupName)
        {
           // this.groupName = groupName;
           CustomShaderUtils.TrytoGetEnableKeyword(groupName,out this.groupName,out this.enableKeyword);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            bool show = editor.IsHeaderShowInInspector(groupName);
            show &= editor.IsShowInInspectorByKeyword(enableKeyword);
            if (show)
            {
                DefaultShaderProperty(position, prop, label);
            }
            else
            {
                GUILayout.Space(-this.GetPropertyHeight(prop, label.text, editor) - 2);
            }
        }

        internal void DefaultShaderProperty(Rect position, MaterialProperty prop, GUIContent label)
        {
            switch (prop.type)
            {
                case MaterialProperty.PropType.Range: // float ranges
                    RangeProperty(position, prop, label);
                    break;
                case MaterialProperty.PropType.Float: // floats
                    FloatProperty(position, prop, label);
                    break;
                case MaterialProperty.PropType.Color: // colors
                    ColorProperty(position, prop, label);
                    break;
                //case MaterialProperty.PropType.Texture: // textures
                //    TextureProperty(position, prop, label.text);
                //    break;
                case MaterialProperty.PropType.Vector: // vectors
                    VectorProperty(position, prop, label.text);
                    break;
                default:
                    GUI.Label(position, "Unknown property type: " + prop.name + ": " + (int)prop.type);
                    break;
            }
        }
        internal float RangeProperty(Rect position, MaterialProperty prop, GUIContent label)
        {
            return DoPowerRangeProperty(position, prop, label);
        }

        internal float FloatProperty(Rect position, MaterialProperty prop, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            float newValue = EditorGUI.FloatField(position, label, prop.floatValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                prop.floatValue = newValue;
            return prop.floatValue;
        }

        internal Color ColorProperty(Rect position, MaterialProperty prop, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            bool isHDR = ((prop.flags & MaterialProperty.PropFlags.HDR) != 0);
            bool showAlpha = true;
            Color newValue = EditorGUI.ColorField(position, label, prop.colorValue, true, showAlpha, isHDR, null);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                prop.colorValue = newValue;

            return prop.colorValue;
        }

        public Vector4 VectorProperty(Rect position, MaterialProperty prop, string label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            // We want to make room for the field in case it's drawn on the same line as the label
            // Set label width to default width (zero) temporarily
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0f;
            Vector4 newValue = EditorGUI.Vector4Field(position, label, prop.vectorValue);
            EditorGUIUtility.labelWidth = oldLabelWidth;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                prop.vectorValue = newValue;
            return prop.vectorValue;
        }


        internal static float DoPowerRangeProperty(Rect position, MaterialProperty prop, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            // For range properties we want to show the slider so we adjust label width to use default width (setting it to 0)
            // See SetDefaultGUIWidths where we set: EditorGUIUtility.labelWidth = GUIClip.visibleRect.width - EditorGUIUtility.fieldWidth - 17;
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0f;
            float newValue = EditorGUI.Slider(position, label, prop.floatValue, prop.rangeLimits.x, prop.rangeLimits.y);
            EditorGUI.showMixedValue = false;
            EditorGUIUtility.labelWidth = oldLabelWidth;
            if (EditorGUI.EndChangeCheck())
                prop.floatValue = newValue;
            return prop.floatValue;
        }

    }
}