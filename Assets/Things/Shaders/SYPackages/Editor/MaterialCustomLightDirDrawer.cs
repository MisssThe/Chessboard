using CustomShaderEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// https://zhuanlan.zhihu.com/p/97256929
/// </summary>
public class MaterialCustomLightDirDrawer : MaterialPropertyDrawer
{
    private readonly string groupName;
    private readonly string enableKeyword;
    float height = 16;
    bool isEditor = false;

    bool starEditor = true;
    GameObject selectGameObj;
    public Quaternion rot = Quaternion.identity;

    MaterialProperty m_prop;

    public MaterialCustomLightDirDrawer(string groupName)
    {
        //this.groupName = groupName;
        CustomShaderUtils.TrytoGetEnableKeyword(groupName, out this.groupName, out this.enableKeyword);
    }

    public MaterialCustomLightDirDrawer()
    {
        
        
    }

    //判断是否为Vector类型
    private bool IsPropertyTypeSuitable(MaterialProperty prop)
    {
        return prop.type == MaterialProperty.PropType.Vector;
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        //如果不是Vector类型，则把unity的默认警告框的高度40
        if (!IsPropertyTypeSuitable(prop))
        {
            return 40f;
        }

        height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, new GUIContent(label));
        return height + 4;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
        bool show = editor.IsHeaderShowInInspector(groupName);
        show &= editor.IsShowInInspectorByKeyword(enableKeyword);
        if (show)
        {
            //如果不是Vector类型，则显示一个警告框
            if (!IsPropertyTypeSuitable(prop))
            {
                GUIContent c = EditorGUIUtility.IconContent("console.erroricon",
                    "LightDir used on a non-Vector property: " + prop.name);
                EditorGUI.LabelField(position, c, EditorStyles.helpBox);
                return;
            }

            EditorGUI.BeginChangeCheck();
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0f;

            Color oldColor = GUI.color;
            if (isEditor) GUI.color = Color.green;

            //绘制属性
            Rect VectorRect = new Rect(position)
            {
                width = position.width - 68f
            };

            Vector3 value = new Vector3(prop.vectorValue.x, prop.vectorValue.y, prop.vectorValue.z);
            value = EditorGUI.Vector3Field(VectorRect, label, value);

            //绘制开关
            Rect TogglegRect = new Rect(position)
            {
                x = position.xMax - 64f,
                //y = (height > 16) ? position.y + 16 : position.y,
                y = position.y + 1,
                width = 60f,
                height = 16
            };
            isEditor = GUI.Toggle(TogglegRect, isEditor, "Set", "Button");

            if (isEditor)
            {
                if (starEditor)
                {
                    m_prop = prop;
                    InitSenceGUI(value);
                }
            }
            else
            {
                if (!starEditor)
                {
                    ClearSenceGUI();
                }
            }

            GUI.color = oldColor;
            EditorGUIUtility.labelWidth = oldLabelWidth;
            if (EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(value.x, value.y, value.z);
            }
        }
        else
        {
            GUILayout.Space(-this.GetPropertyHeight(prop, label, editor) - 2);
        }
    }

    void InitSenceGUI(Vector3 value)
    {
        //Tools.current = Tool.None;
        selectGameObj = Selection.activeGameObject;
        //Vector3 worldDir = selectGameObj.transform.rotation * value;
        Vector3 worldDir = value;
        rot = Quaternion.FromToRotation(Vector3.forward, worldDir);
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        starEditor = false;
    }

    void ClearSenceGUI()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        m_prop = null;
        selectGameObj = null;
        starEditor = true;
    }

    void OnSceneGUI(SceneView senceView)
    {
        if (Selection.activeGameObject != selectGameObj)
        {
            ClearSenceGUI();
            isEditor = false;
            return;
        }

        Vector3 pos = selectGameObj.transform.position;

        rot = Handles.RotationHandle(rot, pos);
        //Vector3 newlocalDir = Quaternion.Inverse(selectGameObj.transform.rotation) *  rot * Vector3.forward;
        Vector3 newDir = rot * Vector3.forward;

        m_prop.vectorValue = new Vector4(newDir.x, newDir.y, newDir.z);

        Handles.color = Color.green;
        Handles.ConeHandleCap(0, pos, rot, HandleUtility.GetHandleSize(pos), EventType.Repaint);
    }
}