using CustomShaderEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace CustomShaderEditor
{
    public class MaterialCustomTextureDrawer : MaterialPropertyDrawer
    {
        private readonly string groupName;
        private readonly string enableKeyword;
        private readonly string _keyword;

        public MaterialCustomTextureDrawer() : this(string.Empty, string.Empty) { }

        public MaterialCustomTextureDrawer(string customHeader) : this(customHeader, string.Empty) { }


        public MaterialCustomTextureDrawer(string groupName, string keyword)
        {
            //this.groupName = groupName;
            CustomShaderUtils.TrytoGetEnableKeyword(groupName,out this.groupName,out this.enableKeyword);
            this._keyword = keyword;
          
        }
        

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            Material material = editor.target as Material;

            bool show = editor.IsHeaderShowInInspector(groupName);
            show &= editor.IsShowInInspectorByKeyword(enableKeyword);
            EditorGUI.BeginChangeCheck();
            if (show)
            {
                editor.TextureProperty(position, prop, label, true);
                //prop.textureValue = editor.TextureProperty(position, prop, label, true);
                GUILayout.Space(52);
            }
            else
            {
                GUILayout.Space(-this.GetPropertyHeight(prop, label, editor)-2);
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (!string.IsNullOrEmpty(_keyword))
                {
                    bool state = prop.textureValue != null;
                    material.SetKeyword(_keyword, state);
                }
            }
        }
        public override void Apply(MaterialProperty prop)
        {
            base.Apply(prop);
            foreach (var target in prop.targets)
            {
                var m = target as Material;
                m.SetKeyword(_keyword /*"_NORMALMAP"*/, prop.textureValue != null); // 创建时设置一次 Keyword
            }
          
        }
    }
}
