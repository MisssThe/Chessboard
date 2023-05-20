using UnityEditor;
using UnityEngine;
using System;

namespace CustomShaderGUI
{
   public class EmissionBakingShaderGUI : ShaderGUI
   {
       private static int s_ControlHash = "EditorTextField".GetHashCode();
       
       public static readonly GUIContent Label_isBakeEmission = new GUIContent("烘焙自发光", "在烘焙地形时烘焙模型的自发光效果");
       
       public static readonly string isBakeEmission = "_isBakeEmission";
       
       protected MaterialProperty Property_bakeEmission;

       public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
       {
           if (materialEditor == null)
               throw new ArgumentNullException("materialEditorIn");
           
           
           Property_bakeEmission = FindProperty(isBakeEmission, properties, false);
           
           //显示原来的材质面板GUI
           materialEditor.SetDefaultGUIWidths();//将fieldWidth和labelWidth设置为PropertiesGUI使用的默认值。
           GUIUtility.GetControlID(s_ControlHash, FocusType.Passive, new Rect(0, 0, 0, 0));
           for (var i = 0; i < properties.Length; i++)
           {//遍历当前材质球的所有属性项
               if ((properties[i].flags & (MaterialProperty.PropFlags.HideInInspector |
                                           MaterialProperty.PropFlags.PerRendererData)) != 0)
                   continue;
               
               float h = materialEditor.GetPropertyHeight(properties[i], properties[i].displayName);//计算当前属性项所需的高度。
               Rect r = EditorGUILayout.GetControlRect(true, h, EditorStyles.layerMaskField);//获取当前属性项的Rect

               materialEditor.ShaderProperty(r, properties[i], properties[i].displayName);//根据当前属性项的Rect参数绘制当前属性项
           }

           GUILayout.Space(8);
           
           //绘制自定义材质GUI
           //烘焙自发光材质面板GUI
           if (Property_bakeEmission != null)
           { 
               EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                        EditorGUI.showMixedValue = Property_bakeEmission.hasMixedValue;
                            var isBakeEmissionEnabled = EditorGUILayout.Toggle(Label_isBakeEmission, Property_bakeEmission.floatValue > 0);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Property_bakeEmission.floatValue = isBakeEmissionEnabled ? 1 : 0;
                    }
                    EditorGUI.showMixedValue = false; 
               EditorGUILayout.EndHorizontal();

               var material = materialEditor.target as Material;
               if (Mathf.Approximately(Property_bakeEmission.floatValue, 1))
               {
                   material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
               }
               else
               {
                   material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
               }
           }
           
           
           //暴露Global Illumination Flags选项
           //materialEditor.LightmapEmissionProperty();
       }
   }
}


