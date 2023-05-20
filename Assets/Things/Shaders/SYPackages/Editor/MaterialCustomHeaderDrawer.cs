using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace CustomShaderEditor
{
  
    public class MaterialCustomHeaderDrawer : MaterialPropertyDrawer
    {
        private readonly string groupName;
        private readonly string enableKeyword;
        private readonly string header;
        private readonly int size;


        public MaterialCustomHeaderDrawer(string groupName) : this(groupName, "", 11) { }
      

        public MaterialCustomHeaderDrawer(string groupName, string header): this(groupName, header, 11) { }
    
        public MaterialCustomHeaderDrawer(string groupName, string header,int size)
        {
           // this.groupName = groupName;
           CustomShaderUtils.TrytoGetEnableKeyword(groupName,out this.groupName,out this.enableKeyword);
            this.header = header;
            this.size = size;
        }

        // so that we can accept Header(1) and display that as text
        public MaterialCustomHeaderDrawer(string groupName, float headerAsNumber)
        {
            //this.groupName = groupName;
            CustomShaderUtils.TrytoGetEnableKeyword(groupName,out this.groupName,out this.enableKeyword);
            this.header = headerAsNumber.ToString(CultureInfo.InvariantCulture);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return 20f;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            bool show = editor.IsHeaderShowInInspector(groupName);
            show &= editor.IsShowInInspectorByKeyword(enableKeyword);
            if (show)
            {
                position.y += 8;
                position = EditorGUI.IndentedRect(position);
                var content = string.IsNullOrEmpty(header) ? label : header;
                //var style = new GUIStyle("ShurikenModuleTitle");
                //style.alignment = TextAnchor.MiddleLeft;
                //style.fontSize = 12; // 默认 9
                //style.contentOffset = new Vector2(10, -10);
                //style.fontStyle = FontStyle.Bold;
                ////style.border = new RectOffset(20, 7, 12,-12);
                //style.overflow= new RectOffset(0, 0, 12, -4);
                //style.normal.textColor = Color.white;
                //GUILayout.Label(content, style);
                CustomShaderGUIUtils.DrawShurikenLabel(content, size);
                // GUILayout.Space(-14);
            }
            else
            {
                GUILayout.Space(-this.GetPropertyHeight(prop, label, editor) - 2);
            }
        }
    }
}