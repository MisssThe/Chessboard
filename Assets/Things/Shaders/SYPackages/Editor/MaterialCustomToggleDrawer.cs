using CustomShaderEditor;
using System;
using UnityEditor;
using UnityEngine;

namespace CustomShaderEditor
{

    internal class MaterialCustomToggleDrawer : MaterialPropertyDrawer
    {
        protected readonly string groupName;
        protected readonly string enableKeyword;
        protected readonly string keyword;

        public MaterialCustomToggleDrawer(string groupName)
        {
            CustomShaderUtils.TrytoGetEnableKeyword(groupName,out this.groupName,out this.enableKeyword);
            //this.groupName = groupName;
        }
        public MaterialCustomToggleDrawer(string groupName, string keyword)
        {
            //this.groupName = groupName;
            CustomShaderUtils.TrytoGetEnableKeyword(groupName,out this.groupName,out this.enableKeyword);
            this.keyword = keyword;
        }

        static bool IsPropertyTypeSuitable(MaterialProperty prop)
        {
            return prop.type == MaterialProperty.PropType.Float || prop.type == MaterialProperty.PropType.Range;
        }

        protected virtual void SetKeyword(MaterialProperty prop, bool on)
        {
            SetKeywordInternal(prop, on, "_ON");
        }

        protected void SetKeywordInternal(MaterialProperty prop, bool on, string defaultKeywordSuffix)
        {
            // if no keyword is provided, use <uppercase property name> + defaultKeywordSuffix
            string kw = string.IsNullOrEmpty(keyword) ? prop.name.ToUpperInvariant() + defaultKeywordSuffix : keyword;
            // set or clear the keyword
            if (!string.IsNullOrEmpty(kw))
            {
                foreach (Material material in prop.targets)
                {
                    if (on)
                        material.EnableKeyword(kw);
                    else
                        material.DisableKeyword(kw);

                
                } 
            }
           
        }
        protected virtual bool CheckIfKeywordIsEnable(MaterialProperty prop)
        {
            string kw = string.IsNullOrEmpty(keyword) ? prop.name.ToUpperInvariant() + "_ON" : keyword;
            bool enable = false;
            foreach (Material material in prop.targets)
            {
                enable |= CheckIfKeywordIsEnable(material, kw);
            }
            return enable;
        }
        protected virtual bool CheckIfKeywordIsEnable(Material material,string keyword)
        {
            return material.IsKeywordEnabled(keyword);
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
            show &= editor.IsShowInInspectorByKeyword(enableKeyword);
            if (show)
            {
                if (!IsPropertyTypeSuitable(prop))
                {
                    GUIContent c = CustomShaderGUIUtils.TempContent("Toggle used on a non-float property: " + prop.name,
                            CustomShaderGUIUtils.GetHelpIcon(MessageType.Warning));
                    EditorGUI.LabelField(position, c, EditorStyles.helpBox);
                    return;
                }

                EditorGUI.BeginChangeCheck();

                //界面展开时先检测是否已经开启了Keyword
               bool isKeywordEnable = CheckIfKeywordIsEnable(prop);

                bool value = (Math.Abs(prop.floatValue) > 0.001f);//|| isKeywordEnable;
                EditorGUI.showMixedValue = prop.hasMixedValue;
                value = EditorGUI.Toggle(position, label, value);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    prop.floatValue = value ? 1.0f : 0.0f;
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

            SetKeyword(prop, (Math.Abs(prop.floatValue) > 0.001f));
        }
    }

    // Variant of the ToggleDrawer that defines a keyword when it's not on
    // This is useful when adding Toggles to existing shaders while maintaining backwards compatibility
    internal class MaterialCustomToggleOffDrawer : MaterialCustomToggleDrawer
    {

        public MaterialCustomToggleOffDrawer(string groupName) : base(groupName)
        {

        }
        public MaterialCustomToggleOffDrawer(string groupName, string keyword) : base(groupName, keyword)
        {

        }
        protected override void SetKeyword(MaterialProperty prop, bool on)
        {
            SetKeywordInternal(prop, !on, "_OFF");
        }
    }
}