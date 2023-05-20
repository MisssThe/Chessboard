using UnityEngine;
using UnityEditor;

namespace GraphicsLib
{
    public class MaterialKeywordTexDrawer : MaterialPropertyDrawer
    {
        string mKeyword = "";

        public MaterialKeywordTexDrawer()
        {
            mKeyword = "";
        }

        public MaterialKeywordTexDrawer(string keyword)
        {
            mKeyword = keyword;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            EditorGUI.BeginChangeCheck();
            prop.textureValue = editor.TextureProperty(position, prop, label, false);
            var state = prop.textureValue != null ? true : false;
            if (EditorGUI.EndChangeCheck())
            {
                SetKeyword(prop, mKeyword, state);
            }
             GUILayout.Space(52);
        }

        public override void Apply(MaterialProperty prop)
        {
            base.Apply(prop);
            SetKeyword(prop, mKeyword /*"_NORMALMAP"*/, prop.textureValue != null); // 创建时设置一次 Keyword
        }

        static void SetKeyword(MaterialProperty prop, string keyword, bool state)
        {
            foreach (var target in prop.targets)
            {
                var m = target as Material;
                if (state)
                    m.EnableKeyword(keyword);
                else
                    m.DisableKeyword(keyword);
            }

        }
    }
}