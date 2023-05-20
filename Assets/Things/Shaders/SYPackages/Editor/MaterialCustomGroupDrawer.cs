using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace CustomShaderEditor
{
    /// <summary>
    /// 折叠Group Header
    /// </summary>
    public class MaterialCustomGroupDrawer : MaterialPropertyDrawer
    {
        bool lastShow = false;

        public string groupname;

        public MaterialCustomGroupDrawer() : this("") { }

        public MaterialCustomGroupDrawer(string groupname)
        {
            this.groupname = groupname;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            Material material = editor.target as Material;
            GUILayout.Space(-12);
            lastShow = CustomShaderGUIUtils.Foldout(lastShow, label.text);
            prop.floatValue = lastShow ? 1 : 0;

        }
    }
}
