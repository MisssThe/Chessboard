using UnityEditor;
using UnityEngine;
namespace CustomShaderEditor
{
    internal class MaterialCustomKeywordEnumDrawer : MaterialPropertyDrawer
    {
        private readonly string groupName;
        private readonly GUIContent[] keywords;

        public MaterialCustomKeywordEnumDrawer(string groupName, string kw1) : this(groupName, new[] { kw1 }) { }
        public MaterialCustomKeywordEnumDrawer(string groupName, string kw1, string kw2) : this(groupName, new[] { kw1, kw2 }) { }
        public MaterialCustomKeywordEnumDrawer(string groupName, string kw1, string kw2, string kw3) : this(groupName, new[] { kw1, kw2, kw3 }) { }
        public MaterialCustomKeywordEnumDrawer(string groupName, string kw1, string kw2, string kw3, string kw4) : this(groupName, new[] { kw1, kw2, kw3, kw4 }) { }
        public MaterialCustomKeywordEnumDrawer(string groupName, string kw1, string kw2, string kw3, string kw4, string kw5) : this(groupName, new[] { kw1, kw2, kw3, kw4, kw5 }) { }
        public MaterialCustomKeywordEnumDrawer(string groupName, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6) : this(groupName, new[] { kw1, kw2, kw3, kw4, kw5, kw6 }) { }
        public MaterialCustomKeywordEnumDrawer(string groupName, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7) : this(groupName, new[] { kw1, kw2, kw3, kw4, kw5, kw6, kw7 }) { }
        public MaterialCustomKeywordEnumDrawer(string groupName, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8) : this(groupName, new[] { kw1, kw2, kw3, kw4, kw5, kw6, kw7, kw8 }) { }
        public MaterialCustomKeywordEnumDrawer(string groupName, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8, string kw9) : this(groupName, new[] { kw1, kw2, kw3, kw4, kw5, kw6, kw7, kw8, kw9 }) { }
        public MaterialCustomKeywordEnumDrawer(string groupName, params string[] keywords)
        {
            this.groupName = groupName;
            this.keywords = new GUIContent[keywords.Length];
            for (int i = 0; i < keywords.Length; ++i)
                this.keywords[i] = new GUIContent(keywords[i]);
        }

        static bool IsPropertyTypeSuitable(MaterialProperty prop)
        {
            return prop.type == MaterialProperty.PropType.Float || prop.type == MaterialProperty.PropType.Range;
        }

        void SetKeyword(MaterialProperty prop, int index)
        {
            for (int i = 0; i < keywords.Length; ++i)
            {
                //string keyword = GetKeywordName(prop.name, keywords[i].text);
                string keyword =keywords[i].text;
                foreach (Material material in prop.targets)
                {
                    if (index == i)
                        material.EnableKeyword(keyword);
                    else
                        material.DisableKeyword(keyword);
                }
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (!IsPropertyTypeSuitable(prop))
            {
                return 16 * 2.5f;
            }
            return base.GetPropertyHeight(prop, label, editor);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            bool show = editor.IsHeaderShowInInspector(groupName);

            if (show)
            {
                if (!IsPropertyTypeSuitable(prop))
                {
                    GUIContent c = CustomShaderGUIUtils.TempContent("KeywordEnum used on a non-float property: " + prop.name,
                            CustomShaderGUIUtils.GetHelpIcon(MessageType.Warning));
                    EditorGUI.LabelField(position, c, EditorStyles.helpBox);
                    return;
                }

                EditorGUI.BeginChangeCheck();

                EditorGUI.showMixedValue = prop.hasMixedValue;
                var value = (int)prop.floatValue;
                value = EditorGUI.Popup(position, label, value, keywords);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    prop.floatValue = value;
                    SetKeyword(prop, value);
                }
            }
            else
            {
                GUILayout.Space(-this.GetPropertyHeight(prop, label.text, editor) - 2);
            }
        }

        public override void Apply(MaterialProperty prop)
        {
            base.Apply(prop);
            if (!IsPropertyTypeSuitable(prop))
                return;

            if (prop.hasMixedValue)
                return;

            SetKeyword(prop, (int)prop.floatValue);
        }

        // Final keyword name: property name + "_" + display name. Uppercased,
        // and spaces replaced with underscores.
        private static string GetKeywordName(string propName, string name)
        {
            string n = propName + "_" + name;
            return n.Replace(' ', '_').ToUpperInvariant();
        }
    }
}