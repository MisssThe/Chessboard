using CustomShaderEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MaterialCustomColorDrawer : MaterialPropertyDrawer
{
    private readonly string groupName;
    private readonly string enableKeyword;
    public MaterialCustomColorDrawer(string groupName)
    {
        CustomShaderUtils.TrytoGetEnableKeyword(groupName,out this.groupName,out this.enableKeyword);
        //this.groupName = groupName;
    }


    public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
        bool show = editor.IsHeaderShowInInspector(groupName);
        show &= editor.IsShowInInspectorByKeyword(enableKeyword);
        if (show)
        {
            editor.ColorProperty(position,prop, label.text);
        }
        else
        {
            GUILayout.Space(-this.GetPropertyHeight(prop, label.text, editor) - 2);
        }
    }

}
