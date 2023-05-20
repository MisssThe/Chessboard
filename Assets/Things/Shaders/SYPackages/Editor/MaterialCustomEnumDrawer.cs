using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace CustomShaderEditor
{
    public class MaterialCustomEnumDrawer : MaterialPropertyDrawer
    {
        private readonly GUIContent[] names;
        private readonly float[] values;

        private readonly string groupName;
        private readonly string enableKeyword;
        private int selIndex;

        public MaterialCustomEnumDrawer(string enumName)
        {
            var loadedTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => GetTypesFromAssembly(x)).ToArray();
            try
            {
                var enumType = loadedTypes.FirstOrDefault(
                        x => x.IsSubclassOf(typeof(Enum)) && (x.Name == enumName || x.FullName == enumName)
                        );
                var enumNames = Enum.GetNames(enumType);
                this.names = new GUIContent[enumNames.Length];
                for (int i = 0; i < enumNames.Length; ++i)
                    this.names[i] = new GUIContent(enumNames[i]);

                var enumVals = Enum.GetValues(enumType);
                values = new float[enumVals.Length];
                for (var i = 0; i < enumVals.Length; ++i)
                    values[i] = (int)enumVals.GetValue(i);
            }
            catch (Exception)
            {
                Debug.LogWarningFormat("Failed to create MaterialEnum, enum {0} not found", enumName);
                throw;
            }
        }

        // name,value,name,value,... pairs: explicit names & values
        public MaterialCustomEnumDrawer(string groupName, string n1, float v1) : this(groupName, new[] { n1 }, new[] { v1 }) { }
        public MaterialCustomEnumDrawer(string groupName, string n1, float v1, string n2, float v2) : this(groupName, new[] { n1, n2 }, new[] { v1, v2 }) { }
        public MaterialCustomEnumDrawer(string groupName, string n1, float v1, string n2, float v2, string n3, float v3) : this(groupName, new[] { n1, n2, n3 }, new[] { v1, v2, v3 }) { }
        public MaterialCustomEnumDrawer(string groupName, string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4) : this(groupName, new[] { n1, n2, n3, n4 }, new[] { v1, v2, v3, v4 }) { }
        public MaterialCustomEnumDrawer(string groupName, string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5) : this(groupName, new[] { n1, n2, n3, n4, n5 }, new[] { v1, v2, v3, v4, v5 }) { }
        public MaterialCustomEnumDrawer(string groupName, string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5, string n6, float v6) : this(groupName, new[] { n1, n2, n3, n4, n5, n6 }, new[] { v1, v2, v3, v4, v5, v6 }) { }
        public MaterialCustomEnumDrawer(string groupName, string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5, string n6, float v6, string n7, float v7) : this(groupName, new[] { n1, n2, n3, n4, n5, n6, n7 }, new[] { v1, v2, v3, v4, v5, v6, v7 }) { }
        public MaterialCustomEnumDrawer(string groupName, string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5, string n6, float v6, string n7, float v7, string n8, float v8) : this(groupName, new[] { n1, n2, n3, n4, n5, n6, n7, n8 }, new[] { v1, v2, v3, v4, v5, v6, v7, v8 }) { }
        public MaterialCustomEnumDrawer(string groupName, string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5, string n6, float v6, string n7, float v7, string n8, float v8, string n9, float v9) : this(groupName, new[] { n1, n2, n3, n4, n5, n6, n7, n8, n9 }, new[] { v1, v2, v3, v4, v5, v6, v7, v8, v9 }) { }
        public MaterialCustomEnumDrawer(string groupName, string[] enumNames, float[] vals)
        {
            this.names = new GUIContent[enumNames.Length];
            for (int i = 0; i < enumNames.Length; ++i)
                this.names[i] = new GUIContent(enumNames[i]);

            values = new float[vals.Length];
            for (int i = 0; i < vals.Length; ++i)
                values[i] = vals[i];

            //this.groupName = groupName;
            CustomShaderUtils.TrytoGetEnableKeyword(groupName,out this.groupName,out this.enableKeyword);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (prop.type != MaterialProperty.PropType.Float && prop.type != MaterialProperty.PropType.Range)
            {
                return 16 * 2.5f;
            }
            return base.GetPropertyHeight(prop, label, editor);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (prop.type != MaterialProperty.PropType.Float && prop.type != MaterialProperty.PropType.Range)
            {
                GUIContent c = CustomShaderGUIUtils.TempContent("Enum used on a non-float property: " + prop.name,
                        CustomShaderGUIUtils.GetHelpIcon(MessageType.Warning));
                EditorGUI.LabelField(position, c, EditorStyles.helpBox);
                return;
            }
            bool show = editor.IsHeaderShowInInspector(groupName);
            show &= editor.IsShowInInspectorByKeyword(enableKeyword);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            var value = prop.floatValue;
            int selectedIndex = -1;
            for (var index = 0; index < values.Length; index++)
            {
                var i = values[index];
                if (i == value)
                {
                    selectedIndex = index;
                    break;
                }
            }
            EditorGUI.showMixedValue = false;
            if (!show)
            {
                GUILayout.Space(-this.GetPropertyHeight(prop, label.text, editor) - 2);
            }
            else
            {
                selIndex = EditorGUI.Popup(position, label, selectedIndex, names);
            }
            if (EditorGUI.EndChangeCheck())
            {
                prop.floatValue = values[selIndex];
            }
        }

        internal static Type[] GetTypesFromAssembly(Assembly assembly)
        {
            if (assembly == null)
                return new Type[] { };
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException)
            {
                return new Type[] { };
            }
        }
    }
}
