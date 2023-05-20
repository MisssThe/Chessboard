using CustomShaderEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomShaderEditor
{
    public class CustomShaderGUINew : ShaderGUI
    {
        public enum SurfaceType
        {
            Opaque,
            Transparent
        }

        public enum RenderFace
        {
            Front = 2,
            Back = 1,
            Both = 0
        }

        protected class PropertyInfo
        {
            public static readonly string Surface = "_Surface";
            public static readonly string Blend = "_Blend";
            public static readonly string Cull = "_Cull";
            public static readonly string AlphaTest = "_AlphaClip";
            public static readonly string Cutoff = "_Cutoff";
            public static readonly string ZTest = "_ZTest";
            public static readonly string ZWrite = "_ZWrite";
            public static readonly string SrcBlend = "_SrcBlend";
            public static readonly string DstBlend = "_DstBlend";
            public static readonly string QueueOffset = "_QueueOffset";
            public static readonly string BakeEmission = "_BakeEmission";
        }

        protected class MaterialKeyWord
        {
            public static readonly string ALPHATEST_ON = "ALPHATEST_ON";
        }

        protected class Styles
        {
            public static readonly GUIContent ResetOptionsLabel =
                new GUIContent("默认渲染设置", "还原为标准的不透明材质");

            public static readonly GUIContent ResetButtonLabel =
                new GUIContent("重设", "");

            public static readonly GUIContent SurfaceOptionsLabel =
                new GUIContent("渲染模式", "控制该材质是不透明还是半透明材质");

            public static readonly string[] SurfaceModeOptions = new string[] {"不透明", "半透明"};

            public static GUIContent RenderFaceOptionsLabel =
                new GUIContent("渲染模式", "用于控制材质的渲染类型");

            public static readonly string[] RenderModeOptions = new string[] {"正面剔除", "背面剔除", "双面显示"};

            public static GUIContent alphaClipText =
                new GUIContent("透明裁剪", "是否开启透明裁剪");

            public static GUIContent alphaClipThresholdText = new GUIContent("裁剪阈值",
                "该值用于控制像素是否被裁剪，只有 alpha 大于该值的像素才不会被裁剪");

            public static GUIContent ZTestModeOptionsLabel = new GUIContent("深度检测",
                "该值用于控制材质的深度判断，非特殊材质不需要手动配置");

            public static readonly string[] ZTestModeOptions = new string[]
            {
                "关闭",
                "总不通过",
                "小于",
                "等于",
                "小于等于",
                "大于",
                "不等于",
                "大于等于",
                "总是通过"
            };

            public static readonly GUIContent ZWriteOptionsLabel = new GUIContent("深度写入",
                "该值用于配置是否进行深度写入，如非必要，不要修改");

            public static readonly string[] ZwriteOptions = new string[] {"关闭", "开启"};

            public static readonly GUIContent BlendOptionLabel = new GUIContent("混合模式",
                "像素的混合模式，如非必要，不要修改");

            public static readonly GUIContent SrcBlendOption = new GUIContent("SrcBlendOption");
            public static readonly GUIContent DstBlendOption = new GUIContent("DstBlendOption");

            public static readonly GUIContent queueSlider = new GUIContent("Queue Offset",
                "Determines the chronological rendering order for a Material. High values are rendered first.");
        }

        #region 渲染状态属性

        // Material Property
        private MaterialEditor materialEditor;
        protected MaterialProperty surfaceTypeProp;
        protected MaterialProperty cullingProp;
        protected MaterialProperty alphaClipProp;
        protected MaterialProperty alphaCutoffProp;
        protected MaterialProperty zTestrop;
        protected MaterialProperty zWriteProp;
        protected MaterialProperty srcBlendProp;
        protected MaterialProperty dstBlendprop;
        protected MaterialProperty queueOffsetProp;
        protected MaterialProperty bakeEmissionProp;

        #endregion

        private static int s_ControlHash = "EditorTextField".GetHashCode();

        private bool shadeSettingshow = false;
        private bool blendModeChanged { get; set; }
        private const int queueOffsetRange = 50;
        
        private SurfaceType oldType = SurfaceType.Opaque;


        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            if (materialEditor == null)
                throw new ArgumentNullException("materialEditorIn");
            // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
            FindProperties(props);
            this.materialEditor = materialEditor;
            var material = materialEditor.target as Material;
            ShaderPropertiesGUI(material, props);
            materialEditor.RenderQueueField();
        }

        public void FindProperties(MaterialProperty[] properties)
        {
            // Surface Option Props
            surfaceTypeProp = FindProp(PropertyInfo.Surface, properties);
            //blendModeProp = FindProp(PropertyInfo.Blend, properties);
            cullingProp = FindProp(PropertyInfo.Cull, properties);
            alphaClipProp = FindProp(PropertyInfo.AlphaTest, properties);
            alphaCutoffProp = FindProp(PropertyInfo.Cutoff, properties);

            zTestrop = FindProp(PropertyInfo.ZTest, properties);
            zWriteProp = FindProp(PropertyInfo.ZWrite, properties);
            srcBlendProp = FindProp(PropertyInfo.SrcBlend, properties);
            dstBlendprop = FindProp(PropertyInfo.DstBlend, properties);
            bakeEmissionProp = FindProp(PropertyInfo.BakeEmission, properties);
            queueOffsetProp = FindProperty(PropertyInfo.QueueOffset, properties, false);
            // _CullMode = FindProp(mCullModeName, properties);
            // _Cutoff = FindProp(mCullOff, properties);
        }


        public static MaterialProperty FindProp(string propertyName, MaterialProperty[] properties,
            bool propertyIsMandatory = false)
        {
            return FindProperty(propertyName, properties, propertyIsMandatory);
        }

        public void ShaderPropertiesGUI(Material material, MaterialProperty[] properties)
        {
            EditorGUI.BeginChangeCheck();
            CustomPropertiesGUI(properties);
            ShadeSettingGUI(material);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in materialEditor.targets)
                    MaterialChanged((Material) obj);
            }

            EditorGUILayout.Space();
            
        }

        public void CustomPropertiesGUI(MaterialProperty[] props)
        {
            materialEditor.SetDefaultGUIWidths();
            GUIUtility.GetControlID(s_ControlHash, FocusType.Passive, new Rect(0, 0, 0, 0));
            for (var i = 0; i < props.Length; i++)
            {
                if ((props[i].flags & (MaterialProperty.PropFlags.HideInInspector |
                                       MaterialProperty.PropFlags.PerRendererData)) != 0)
                    continue;
                float h = materialEditor.GetPropertyHeight(props[i], props[i].displayName);
                Rect r = EditorGUILayout.GetControlRect(true, h, EditorStyles.layerMaskField);

                materialEditor.ShaderProperty(r, props[i], props[i].displayName);
            }

            GUILayout.Space(8);
        }

        private void ShadeSettingGUI(Material material)
        {
            shadeSettingshow = CustomShaderGUIUtils.Foldout(shadeSettingshow, "渲染设置");
            if (shadeSettingshow)
            {
                ResetGUI(material, materialEditor);
                SurfaceGUI(material, materialEditor);
                AlphaClipGUI(material, materialEditor);
                RenderFaceGUI(material, materialEditor);
                ZTestGUI(material, materialEditor);
                ZWriteGUI(material, materialEditor);
                this.materialEditor.EnableInstancingField();
                //this.materialEditor.RenderQueueField();
                DrawAdvancedOptions(material, materialEditor);
                BlendGUI(material, materialEditor);
                GlobalIllumination(material, materialEditor);
                
                //materialEditor.LightmapEmissionProperty();
            }
        }

        void SurfaceGUI(Material mat, MaterialEditor editor)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.showMixedValue = surfaceTypeProp.hasMixedValue;
            var mode = surfaceTypeProp.floatValue;
            EditorGUI.BeginChangeCheck();
            mode = EditorGUILayout.Popup(Styles.SurfaceOptionsLabel, (int) mode, Styles.SurfaceModeOptions);
            blendModeChanged = false;
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(Styles.SurfaceOptionsLabel.text);
                surfaceTypeProp.floatValue = mode;
                blendModeChanged = true;
            }

            EditorGUI.showMixedValue = false;
            EditorGUILayout.EndHorizontal();
        }

        void AlphaClipGUI(Material mat, MaterialEditor editor)
        {
            if (alphaClipProp != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = alphaClipProp.hasMixedValue;
                var alphaClipEnabled = EditorGUILayout.Toggle(Styles.alphaClipText, alphaClipProp.floatValue == 1);
                if (EditorGUI.EndChangeCheck())
                {
                    alphaClipProp.floatValue = alphaClipEnabled ? 1 : 0;
                }

                EditorGUI.showMixedValue = false;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (Mathf.Approximately(alphaClipProp.floatValue, 1))
                    materialEditor.ShaderProperty(alphaCutoffProp, Styles.alphaClipThresholdText, 0);
                EditorGUILayout.EndHorizontal();   
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                if (EditorGUI.EndChangeCheck())
                {
                    
                }
                materialEditor.ShaderProperty(alphaCutoffProp, Styles.alphaClipThresholdText, 0);
                EditorGUILayout.EndHorizontal();   
            }
        }

        void RenderFaceGUI(Material material, MaterialEditor editor)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.showMixedValue = cullingProp.hasMixedValue;
            var culling = (RenderFace) cullingProp.floatValue;
            EditorGUI.BeginChangeCheck();
            culling = (RenderFace) EditorGUILayout.EnumPopup(Styles.RenderFaceOptionsLabel, culling);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(Styles.RenderFaceOptionsLabel.text);
                cullingProp.floatValue = (float) culling;
                material.doubleSidedGI = (RenderFace) cullingProp.floatValue != RenderFace.Front;
            }

            EditorGUI.showMixedValue = false;
            EditorGUILayout.EndHorizontal();
        }

        void ZTestGUI(Material mat, MaterialEditor editor)
        {
            EditorGUILayout.BeginHorizontal();
            var ztest = zTestrop.floatValue;
            EditorGUI.BeginChangeCheck();
            ztest = EditorGUILayout.Popup(Styles.ZTestModeOptionsLabel, (int) ztest, Styles.ZTestModeOptions);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(Styles.ZTestModeOptionsLabel.text);
                zTestrop.floatValue = ztest;
            }

            EditorGUILayout.EndHorizontal();
        }

        void ZWriteGUI(Material mat, MaterialEditor editor)
        {
            EditorGUILayout.BeginHorizontal();
            var zwrite = zWriteProp.floatValue;
            EditorGUI.BeginChangeCheck();
            zwrite = EditorGUILayout.Popup(Styles.ZWriteOptionsLabel, (int) zwrite, Styles.ZwriteOptions);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(Styles.ZWriteOptionsLabel.text);
                zWriteProp.floatValue = zwrite;
            }

            EditorGUILayout.EndHorizontal();
        }

        public virtual void DrawAdvancedOptions(Material material, MaterialEditor editor)
        {
            if (queueOffsetProp != null)
            {
                //EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = queueOffsetProp.hasMixedValue;
                //queueOffsetProp.floatValue = EditorGUILayout.Slider(Styles.queueSlider,queueOffsetProp.floatValue, -10, 10);
                GUILayout.Label(Styles.queueSlider);
                GUILayout.FlexibleSpace();
                var queue = EditorGUILayout.IntSlider( (int) queueOffsetProp.floatValue, -queueOffsetRange,
                    queueOffsetRange);
                if (EditorGUI.EndChangeCheck())
                    queueOffsetProp.floatValue = queue;
                EditorGUI.showMixedValue = false;
                //EditorGUILayout.EndHorizontal();
            }
        }

        void BlendGUI(Material mat, MaterialEditor editor)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            // var style = new GUIStyle("ShurikenLabel");
            // style.alignment = TextAnchor.MiddleLeft;
            // style.fontSize = 11; // 默认 9
            // style.fontStyle = FontStyle.Bold;
            // style.normal.textColor = Color.white;
            //GUILayout.Label(Styles.BlendOptionLabel, "BoldLabel");
            GUILayout.Label(Styles.BlendOptionLabel);
            var scr = (BlendMode) srcBlendProp.floatValue;
            EditorGUI.BeginChangeCheck();
            scr = (BlendMode) EditorGUILayout.EnumPopup(scr);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(Styles.SrcBlendOption.text);
                srcBlendProp.floatValue = (float) scr;
            }

            var dst = (BlendMode) dstBlendprop.floatValue;
            EditorGUI.BeginChangeCheck();
            dst = (BlendMode) EditorGUILayout.EnumPopup(dst);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(Styles.DstBlendOption.text);
                dstBlendprop.floatValue = (float) dst;
            }

            EditorGUILayout.EndHorizontal();
        }

        void GlobalIllumination(Material mat, MaterialEditor editor)
        {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUI.BeginChangeCheck();

            if (bakeEmissionProp != null)
            {
                var bakeEmission = EditorGUILayout.Toggle("烘焙自发光", bakeEmissionProp.floatValue == 1);
                if (EditorGUI.EndChangeCheck())
                {
                    bakeEmissionProp.floatValue = bakeEmission ? 1 : 0;
                    if (bakeEmission)
                    {
                        var material = materialEditor.target as Material;
                        if (material != null)
                            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                    }
                    else
                    {
                        var material = materialEditor.target as Material;
                        if (material != null)
                            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                    }
                }   
            }

            EditorGUILayout.EndHorizontal();
        }
        
        protected void ResetGUI(Material mat, MaterialEditor editor)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            GUILayout.Label(Styles.ResetOptionsLabel);
            GUILayout.FlexibleSpace();
            var reset = GUILayout.Button(Styles.ResetButtonLabel, GUILayout.MinWidth(65), GUILayout.Height(15),
                GUILayout.ExpandWidth(true));
            if (reset)
            {
                Reset(mat);
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Reset Shading Setting  as "Opaque"
        /// </summary>
        /// <param name="mat"></param>
        protected void Reset(Material mat)
        {
            surfaceTypeProp.floatValue = (float) SurfaceType.Opaque;
            cullingProp.floatValue = (float) RenderFace.Front; //渲染模式，渲染正面
            alphaClipProp.floatValue = 0;
            alphaCutoffProp.floatValue = 0.5f;

            zTestrop.floatValue = (float) CompareFunction.LessEqual; //深度测试， 小于等于
            zWriteProp.floatValue = 1; //深度写入
            var one = (int) UnityEngine.Rendering.BlendMode.One;
            var zero = (int) UnityEngine.Rendering.BlendMode.Zero;
            srcBlendProp.floatValue = one;
            dstBlendprop.floatValue = zero;
            bakeEmissionProp.floatValue = one;

            mat.doubleSidedGI = false; //烘焙属性修改
        }

        //
        //
        // private float EditorGUILayoutFloatSlider(string desc, float val, float min, float max)
        // {
        //     EditorGUILayout.BeginHorizontal();
        //     GUILayout.Label(desc, GUILayout.ExpandWidth(false));
        //     val = GUILayout.HorizontalSlider(val, min, max, GUILayout.ExpandWidth(true));
        //     val = EditorGUILayout.FloatField(val, GUILayout.Width(64));
        //     EditorGUILayout.EndHorizontal();
        //     return val;
        // }
        //
        // material changed check
        public void MaterialChanged(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");
            

            SetMaterialBlendMode(material);
        }
        private void SetMaterialBlendMode(Material material)
        {

            if (material == null)
                throw new ArgumentNullException("material");

            bool alphaClip = false;
            if (material.HasProperty(PropertyInfo.AlphaTest))
                alphaClip = material.GetFloat(PropertyInfo.AlphaTest) >= 0.5;

            if (alphaClip)
            {
                material.EnableKeyword("_ALPHATEST_ON");
            }
            else
            {
                material.DisableKeyword("_ALPHATEST_ON");
            }

            if (material.HasProperty(PropertyInfo.Surface))
            {
                SurfaceType surfaceType = (SurfaceType) material.GetFloat(PropertyInfo.Surface);
                if (oldType != surfaceType)
                {
                    oldType = surfaceType;
                    if (surfaceType == SurfaceType.Opaque)
                    {
                        if (alphaClip)
                        {
                            material.renderQueue = (int) RenderQueue.AlphaTest;
                            material.SetOverrideTag("RenderType", "TransparentCutout");
                            //material.SetOverrideTag("RenderCustom", "CharacterTransparentCutout");
                        }
                        else
                        {
                            material.renderQueue = (int) RenderQueue.Geometry;
                            material.SetOverrideTag("RenderType", "Opaque");
                            //material.SetOverrideTag("RenderCustom", "CharacterOpaque");
                        }

                        material.renderQueue += material.HasProperty(PropertyInfo.QueueOffset)
                            ? (int) material.GetFloat("_QueueOffset")
                            : 0;
                        if (blendModeChanged)
                        {
                            material.SetInt(PropertyInfo.SrcBlend, (int) UnityEngine.Rendering.BlendMode.One);
                            material.SetInt(PropertyInfo.DstBlend, (int) UnityEngine.Rendering.BlendMode.Zero);
                            material.SetInt(PropertyInfo.ZWrite, 1);
                        }

                        material.SetShaderPassEnabled("ShadowCaster", true);
                    }
                    else
                    {
                        // General Transparent Material Settings
                        material.SetOverrideTag("RenderType", "Transparent");
                        //material.SetOverrideTag("RenderCustom", "CharacterTransparent");
                        // BlendMode blendMode = (BlendMode) material.GetFloat("_Blend");
                        if (blendModeChanged)
                        {
                            material.SetInt(PropertyInfo.SrcBlend, (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                            material.SetInt(PropertyInfo.DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            material.SetInt(PropertyInfo.ZWrite, 0);
                        }

                        material.renderQueue = (int) RenderQueue.Transparent;
                        material.renderQueue += material.HasProperty(PropertyInfo.QueueOffset)
                            ? (int) material.GetFloat("_QueueOffset")
                            : 0;
                        material.SetShaderPassEnabled("ShadowCaster", false);
                    }
                }
                
            }

            blendModeChanged = false;
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            base.AssignNewShaderToMaterial(material, oldShader, newShader);
            // if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            // {
            //     SetMaterialBlendMode(material);
            //     return;
            // }
            MaterialChanged(material);
        }
    }
}