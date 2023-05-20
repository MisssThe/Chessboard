//机战项目特效通用材质v1.0 -2023.2.6
//机战项目特效通用材质v1.1 -2023.2.6 修复溶解材质在模型上自定义数据计算错误问题
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EffectUniversal_GUI : ShaderGUI
{
    //自定义下拉菜单的形状属性
    static bool Foldout(bool display, string title)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = 22;
        style.contentOffset = new Vector2(20f, -2f);
        style.fontSize = 11;
        style.normal.textColor = new Color(0.7f, 0.8f, 0.9f);


        var rect = GUILayoutUtility.GetRect(16f, 25f, style);
        GUI.Box(rect, title, style);

        var e = Event.current;

        var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
        if (e.type == EventType.Repaint)
        {
            EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        }

        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            display = !display;
            e.Use();
        }

        return display;
    }


     enum BlendMode
    {
        AlphaBlend, Add
    }
    string[] blendModeNames = System.Enum.GetNames(typeof(BlendMode));

    void SetupBlentMode(Material targetMat)
    {
        int value = (int)targetMat.GetFloat("_BlendTemp");
        switch (value)
        {
            case 0:
                targetMat.SetInt("_BlendModeSrc", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                targetMat.SetInt("_BlendModeDst", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                break;
            case 1:
                targetMat.SetInt("_BlendModeSrc", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                targetMat.SetInt("_BlendModeDst", (int)UnityEngine.Rendering.BlendMode.One);
                break;
        }
    }


    //自定义变量
    static bool _Base_Foldout = true;
    static bool _Maintextures_Foldout = true;
    static bool _Fresnel_Foldout = true;
    static bool _Masktextures_Foldout = true;
    static bool _Noisetextures_Foldout = true;
    static bool _Dissolve_Foldout = true;
    static bool _Flowmap_Flodout = true;
    static bool _Sub_Flodout = true;
    static bool ismainCustom = false;
    static bool isMaskON = false;
    static bool ismaskCustom = false;
    static bool isNoiseON = false;
    static bool isNoiseCustom = false;
    static bool isDissolveON = false;
    static bool isvoOn = false;
    static bool isuseUV2 = false;
    static bool isMainRota = false;
    static bool isSoft = false;
    static bool isflowmap = false;
    static bool isPolar = false;
    static bool ismaskRota = false;
    static bool isSub = false;
    static bool isSubRota = false;
    static bool isSubUV2 = false;
    static bool isMaskUV2 = false;
    static bool isNoiseUV2 = false;
    static bool isDissolveUV2 = false;
    static bool isDissRota = false;
    static bool isOneMinusFresnel = false;
    static bool isMainClamp = false;
    static bool isMaskClamp = false;
    static bool isSubClamp = false;
    static bool isFresnel = false;
    static bool isDissCustom = false;
    int index = 0;

    //材质编辑器
    MaterialEditor m_MaterialEditor;

    //自定义主要贴图显示的属性
    MaterialProperty mainTex_Sampler = null;
    MaterialProperty mainColor = null;
    MaterialProperty cullMode;
    MaterialProperty zwrite;
    MaterialProperty useUV2;
    MaterialProperty blendTempProp, srcBlendProp, dstBlendProp;
    MaterialProperty softon;
    MaterialProperty distance;
    MaterialProperty mainRota;
    MaterialProperty Rotation;
    MaterialProperty RotaSpeed;
    MaterialProperty mainSpeedU;
    MaterialProperty mainSpeedV;
    MaterialProperty mainCustom;
    MaterialProperty subOn;
    MaterialProperty subColor;
    MaterialProperty sub_Sampler;
    MaterialProperty subLerp;
    MaterialProperty subSpeedU;
    MaterialProperty subSpeedV;
    MaterialProperty subRota;
    MaterialProperty subRotation;
    MaterialProperty subRotaSpeed;
    MaterialProperty Fresnelon;
    MaterialProperty FresnelColor;
    MaterialProperty FresnelBias;
    MaterialProperty FresnelScale;
    MaterialProperty FresnelPower;
    MaterialProperty maskTex_Sampler;
    MaterialProperty maskon;
    MaterialProperty maskTexAlphaChannel;
    MaterialProperty maskSpeedU;
    MaterialProperty maskSpeedV;
    MaterialProperty maskCustom;
    MaterialProperty maskRota;
    MaterialProperty maskRotation;
    MaterialProperty maskRotaSpeed;
    MaterialProperty flowmapOn;
    MaterialProperty flowmap_Sampler;
    MaterialProperty flowmapLerp;
    MaterialProperty noiseon;
    MaterialProperty Noise_Sampler;
    MaterialProperty NoiseInt;
    MaterialProperty noiseSpeedU;
    MaterialProperty noiseSpeedV;
    MaterialProperty noiseCustom;
    MaterialProperty noisemaskTex;
    MaterialProperty Dissolveon;
    MaterialProperty DissolveTexAplha;
    MaterialProperty DissTex_Sampler;
    MaterialProperty DissolveAmount;
    MaterialProperty DissolveSpeedU;
    MaterialProperty DissolveSpeedV;
    MaterialProperty DissolveColor;
    MaterialProperty DissolveCustom;
    MaterialProperty OutLineInt;
    MaterialProperty BlendInt;
    MaterialProperty DissDirTex;
    MaterialProperty DissDirIntensity;
    MaterialProperty subuv2;
    MaterialProperty maskuv2;
    MaterialProperty noiseuv2;
    MaterialProperty dissolveuv2;
    MaterialProperty dissRotation;
    MaterialProperty dissRota;
    MaterialProperty OMfresenl;
    MaterialProperty mainClamp;
    MaterialProperty maskClamp;
    MaterialProperty subClmap;
    MaterialProperty subBlend;
    MaterialProperty flowClamp;


    //将自定义的需要显示的属性指向Shader里的相应变量
    public void FindProperties(MaterialProperty[] props)
    {
        srcBlendProp = FindProperty("_BlendModeSrc", props);
        dstBlendProp = FindProperty("_BlendModeDst", props);
        cullMode = FindProperty("_CullMode",props);
        zwrite = FindProperty("_ZWrite",props);
        useUV2 = FindProperty("_UseUV2",props);
        blendTempProp = FindProperty("_BlendTemp", props);
        softon = FindProperty("_softParticle",props);
        distance = FindProperty("_Distance",props);
        mainTex_Sampler = FindProperty("_MainTex",props);
        mainColor = FindProperty("_MainColor",props);
        mainSpeedU = FindProperty("_MainTex_PannerSpeedU",props);
        mainSpeedV = FindProperty("_MainTex_PannerSpeedV",props);
        mainRota = FindProperty("_MainRota",props);
        Rotation = FindProperty("_Rotation",props);
        RotaSpeed = FindProperty("_RotaSpeed",props);
        subOn = FindProperty("_Sub_ON",props);
        subColor = FindProperty("_SubColor",props);
        sub_Sampler = FindProperty("_SubTex",props);
        subLerp = FindProperty("_SubLerp",props);
        subSpeedU = FindProperty("_SubTex_PannerSpeedU",props);
        subSpeedV = FindProperty("_SubTex_PannerSpeedV",props);
        subRota = FindProperty("_SubRota",props);
        subRotation = FindProperty("_SubRotation",props);
        subRotaSpeed = FindProperty("_SubRotaSpeed",props);
        subBlend = FindProperty("_SubBlend",props);
        Fresnelon = FindProperty("_Fresnel",props);
        FresnelColor = FindProperty("_FresnelColor",props);
        FresnelBias = FindProperty("_FresnelBias",props);
        FresnelScale = FindProperty("_FresnelScale",props);
        FresnelPower = FindProperty("_FresnelPower",props);
        mainCustom = FindProperty("_MainCustom",props);
        maskon = FindProperty("_Mask_ON", props);
        maskTex_Sampler = FindProperty("_MaskTex",props);
        maskTexAlphaChannel = FindProperty("_MaskTexAlpha", props);
        maskSpeedU = FindProperty("_MaskTex_PannerSpeedU",props);
        maskSpeedV = FindProperty("_MaskTex_PannerSpeedV",props);
        maskCustom = FindProperty("_MaskCustom",props);
        maskRota = FindProperty("_MaskRota",props);
        maskRotation = FindProperty("_MaskRotation",props);
        maskRotaSpeed = FindProperty("_MaskRotaSpeed",props);
        flowmapOn = FindProperty("_FlowMap_ON",props);
        flowmap_Sampler = FindProperty("_FlowMap",props);
        flowmapLerp = FindProperty("_FlowLerp",props);
        noiseon = FindProperty("_NOISE_ON",props);
        NoiseInt = FindProperty("_NoiseIntensity",props);
        Noise_Sampler = FindProperty("_NoiseTex",props);
        noiseSpeedU = FindProperty("_NoiseTex_PannerSpeedU",props);
        noiseSpeedV = FindProperty("_NoiseTex_PannerSpeedV",props);
        noiseCustom = FindProperty("_NoiseCustom",props);
        noisemaskTex = FindProperty("_NoiseMaskTex", props);
        Dissolveon = FindProperty("_DissolveMode",props);
        DissTex_Sampler = FindProperty("_DissolveTex",props);
        DissolveAmount = FindProperty("_Dissolve_Amount",props);
        DissolveTexAplha = FindProperty("_DissolveTexAlpha",props);
        DissolveSpeedU = FindProperty("_DissolveTex_PannerSpeedU",props);
        DissolveSpeedV = FindProperty("_DissolveTex_PannerSpeedV",props);
        DissolveColor = FindProperty("_Dissolve_color",props);
        OutLineInt = FindProperty("_OutlineIntensity",props);
        BlendInt = FindProperty("_BlendIntensity",props);
        DissDirTex = FindProperty("_DissolveDirTex", props);
        DissDirIntensity = FindProperty("_DissolveDirIntensity", props);
        DissolveCustom = FindProperty("_DissolveCustom",props);
        subuv2 = FindProperty("_SubUV2",props);
        maskuv2 = FindProperty("_MaskUV2",props);
        noiseuv2 = FindProperty("_NoiseUV2",props);
        dissolveuv2 = FindProperty("_DissolveUV2",props);
        dissRota = FindProperty("_DissRota",props);
        dissRotation = FindProperty("_DissRotation",props);
        OMfresenl = FindProperty("_OneMinusFresnel",props);
        mainClamp = FindProperty("_MainClamp",props);
        subClmap = FindProperty("_SubClamp",props);
        maskClamp = FindProperty("_MaskClamp",props);
        
    }

    //将上面自定义的属性显示在面板上
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        FindProperties(props); 

        m_MaterialEditor = materialEditor;

        Material material = materialEditor.target as Material;


        //基础设置下拉菜单
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _Base_Foldout = Foldout(_Base_Foldout, "基础设置(BasicSettings)");

        if (_Base_Foldout)
        {
            EditorGUI.indentLevel++;
            GUI_Base(material);
            materialEditor.RenderQueueField();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();


        //主贴图下拉菜单
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _Maintextures_Foldout = Foldout(_Maintextures_Foldout, "主贴图(MainColor)");
        if (_Maintextures_Foldout)
        {
            EditorGUI.indentLevel++;

            GUI_Maintexture(material);
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();

        //副贴图下拉菜单
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _Sub_Flodout = Foldout(_Sub_Flodout, "副贴图(SubColor)");
        if (_Sub_Flodout)
        {
            EditorGUI.indentLevel++;

            GUI_Subtexture(material);
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();


        //遮罩下拉菜单
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _Masktextures_Foldout = Foldout(_Masktextures_Foldout, "遮罩贴图(Mask)");
        if (_Masktextures_Foldout)
        {
            EditorGUI.indentLevel++;

            GUI_Masktexture(material);
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();

        //扰动下拉菜单
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _Noisetextures_Foldout = Foldout(_Noisetextures_Foldout, "扰动贴图(Noise)");
        if (_Noisetextures_Foldout)
        {
            EditorGUI.indentLevel++;

            GUI_Noisetexture(material);
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();


        
        //溶解下拉菜单
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _Dissolve_Foldout = Foldout(_Dissolve_Foldout, "溶解贴图(Dissolve)");
        if (_Dissolve_Foldout)
        {
            EditorGUI.indentLevel++;

            GUI_Dissolve(material);
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();

        //FlowMap下拉菜单
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _Flowmap_Flodout = Foldout(_Flowmap_Flodout, "流动插值(FlowMap)");
        if (_Flowmap_Flodout)
        {
            EditorGUI.indentLevel++;

            GUI_FlowMap(material);
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
        
        //边缘光下拉菜单
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _Fresnel_Foldout = Foldout(_Fresnel_Foldout, "菲涅尔(Fresnel)");
        if (_Fresnel_Foldout)
        {
            EditorGUI.indentLevel++;

            GUI_Fresnel(material);
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
        

    }

    void GUI_Base(Material material)
    {
        EditorGUI.BeginChangeCheck();
        blendTempProp.floatValue = EditorGUILayout.Popup("混合模式(BlendMode)", (int)blendTempProp.floatValue, blendModeNames);
        if (EditorGUI.EndChangeCheck())
        {
            SetupBlentMode(material);
        }
        GUILayout.Space(5);

        m_MaterialEditor.ShaderProperty(cullMode, "剔除模式（CullMode）");
        GUILayout.Space(5);

        m_MaterialEditor.ShaderProperty(zwrite, "深度写入(ZWrite)");
        GUILayout.Space(5);


        isSoft = softon.floatValue > 0;
        isSoft = EditorGUILayout.Toggle("是否使用软粒子", isSoft);
        if(isSoft)
        {
            material.SetInt("_softParticle",1);
            material.EnableKeyword("_SOFT_PARTICLE_ON");
            m_MaterialEditor.ShaderProperty(distance, "深度距离");
            GUILayout.Space(5);
            EditorGUILayout.HelpBox("使用软粒子需要打开渲染管线与摄像机深度图", MessageType.None);
        }
        else
        {
            material.SetInt("_softParticle",0);
            material.DisableKeyword("_SOFT_PARTICLE_ON");
        }
        GUILayout.Space(5);

    }

    public void GUI_Maintexture(Material material)
    {
        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("主贴图"), mainTex_Sampler,mainColor);

        if(mainTex_Sampler.textureValue != null)
        {
            m_MaterialEditor.TextureScaleOffsetProperty(mainTex_Sampler);

            isMainClamp = mainClamp.floatValue > 0;
            isMainClamp = EditorGUILayout.Toggle("是否使用纹理阻断", isMainClamp);
            if(isMainClamp)
            {
                material.SetInt("_MainClamp",1);
            }
            else
            {
                material.SetInt("_MainClamp",0);
            }


            isuseUV2 = useUV2.floatValue > 0;
            isuseUV2 = EditorGUILayout.Toggle("是否使用UV2", isuseUV2);
            if(isuseUV2)
            {
                material.SetInt("_UseUV2",1);
            }
            else
            {
                material.SetInt("_UseUV2",0);
            }

            isMainRota = mainRota.floatValue > 0;
            isMainRota = EditorGUILayout.Toggle("是否启用主纹理旋转", isMainRota);
            if(isMainRota)
            {
                material.SetInt("_MainRota", 1);
                
                EditorGUILayout.HelpBox("开启主纹理旋转时将无法使用主纹理流动", MessageType.None);
                GUILayout.Space(5);

                m_MaterialEditor.ShaderProperty(Rotation, "主纹理旋转角度");
                GUILayout.Space(5);

                m_MaterialEditor.ShaderProperty(RotaSpeed, "主纹理旋转速度");
                GUILayout.Space(5);
            }
            else
            {

                material.SetInt("_MainRota", 0);

                
            }


            ismainCustom = mainCustom.floatValue > 0;
            ismainCustom = EditorGUILayout.Toggle("是否启用自定义曲线", ismainCustom);
            GUILayout.Space(5);

            if (ismainCustom)
            {
                material.SetInt("_MainCustom", 1);
                EditorGUILayout.HelpBox("CustomData1.xy控制主纹理UV偏移,如果使用将自定义数据中依次添加UV2，Custom1.xyzw,Custom2.xyzw", MessageType.None);
                GUILayout.Space(5);
                
            }
            else
            {
                material.SetInt("_MainCustom", 0);
                m_MaterialEditor.ShaderProperty(mainSpeedU, "U流动速度");
                GUILayout.Space(5);

                m_MaterialEditor.ShaderProperty(mainSpeedV, "V流动速度");
                GUILayout.Space(5);
            }

        }
         

    }

    public void GUI_Subtexture(Material material)
    {
        isSub = subOn.floatValue > 0 ;
        isSub = EditorGUILayout.Toggle("是否启用副贴图", isSub);
        GUILayout.Space(5);
        if(isSub)
        {
            material.SetInt("_Sub_ON", 1);
            material.EnableKeyword("_Sub_ON");
            m_MaterialEditor.TexturePropertySingleLine(new GUIContent("副贴图"), sub_Sampler,subColor);

            if(sub_Sampler.textureValue != null)
            {
                m_MaterialEditor.TextureScaleOffsetProperty(sub_Sampler);


                isSubUV2 = subuv2.floatValue > 0;
                isSubUV2 = EditorGUILayout.Toggle("是否使用UV2", isSubUV2);
                if(isSubUV2)
                {
                    material.SetInt("_SubUV2",1);
                }
                else
                {
                    material.SetInt("_SubUV2",0);
                }

                isSubClamp = subClmap.floatValue > 0;
                isSubClamp = EditorGUILayout.Toggle("是否使用纹理阻断", isSubClamp);
                if(isSubClamp)
                {
                    material.SetInt("_SubClamp",1);
                }
                else
                {
                    material.SetInt("_SubClamp",0);
                }


                m_MaterialEditor.ShaderProperty(subBlend, "混合模式选择");
                m_MaterialEditor.ShaderProperty(subLerp, "插值通道选择");



                isSubRota = subRota.floatValue > 0;
                isSubRota = EditorGUILayout.Toggle("是否启用副纹理旋转", isSubRota);
                if(isSubRota)
                {
                    material.SetInt("_SubRota", 1);
                    
                    EditorGUILayout.HelpBox("开启副纹理旋转时将无法使用副纹理流动", MessageType.None);
                    GUILayout.Space(5);

                    m_MaterialEditor.ShaderProperty(subRotation, "副纹理旋转角度");
                    GUILayout.Space(5);

                    m_MaterialEditor.ShaderProperty(subRotaSpeed, "副纹理旋转速度");
                    GUILayout.Space(5);
                }
                 else
                {

                    material.SetInt("_SubRota", 0);


                }

                    m_MaterialEditor.ShaderProperty(subSpeedU, "U流动速度");
                    GUILayout.Space(5);

                    m_MaterialEditor.ShaderProperty(subSpeedV, "V流动速度");
                    GUILayout.Space(5);



            }
 
        }
        else
        {
                       
            material.SetInt("_Sub_ON", 0);
            material.DisableKeyword("_Sub_ON");
        }
        
        
        

    }


    public void GUI_FlowMap(Material material)
    {
        isflowmap = flowmapOn.floatValue > 0 ;
        isflowmap = EditorGUILayout.Toggle("是否启用FlowMap", isflowmap);
        GUILayout.Space(5);
        if(isflowmap)
        {
            material.SetInt("_FlowMap_ON", 1);
            material.EnableKeyword("_FlowMap_ON");

            EditorGUILayout.HelpBox("根据FlowMap制作环境调整SRGB", MessageType.None);
            GUILayout.Space(5);
            m_MaterialEditor.TexturePropertySingleLine(new GUIContent("FlowMap"), flowmap_Sampler);

            
            GUILayout.Space(5);

            EditorGUILayout.HelpBox("Custom2.z可以控制FlowMapLerp,使用自定义数据时记得保证FlowMapLerp为0", MessageType.None);
            GUILayout.Space(5);

            m_MaterialEditor.ShaderProperty(flowmapLerp, "FlowMapLerp");
            GUILayout.Space(5);

        }
        else
        {
            material.SetInt("_FlowMap_ON", 0);
            material.DisableKeyword("_FlowMap_ON");
        }
    }


    public void GUI_Fresnel(Material material)
    {
        isFresnel = Fresnelon.floatValue > 0 ;
        isFresnel = EditorGUILayout.Toggle("是否启用菲涅尔", isFresnel);
        if(isFresnel)
        {
            material.SetFloat("_Fresnel",1);
            material.EnableKeyword("_Fresnel_ON");
            m_MaterialEditor.ShaderProperty(FresnelColor, "边缘光颜色");
            GUILayout.Space(5);

            m_MaterialEditor.ShaderProperty(FresnelBias, "底色亮度");
            GUILayout.Space(5);

            m_MaterialEditor.ShaderProperty(FresnelScale, "边缘光亮度");
            GUILayout.Space(5);

            m_MaterialEditor.ShaderProperty(FresnelPower, "边缘光宽度");
            GUILayout.Space(5);

            isOneMinusFresnel = OMfresenl.floatValue > 0 ;
            isOneMinusFresnel = EditorGUILayout.Toggle("是否启用反向菲涅尔", isOneMinusFresnel);
            if(isOneMinusFresnel)
            {
                material.SetFloat("_OneMinusFresnel",1);
            }
            else
            {
                material.SetFloat("_OneMinusFresnel",0);
            }
        }
        else
        {
            material.SetFloat("_Fresnel",0);
            material.DisableKeyword("_Fresnel_ON");
            GUILayout.Space(5);
        }

    }

    public void GUI_Masktexture(Material material)
    {
        
        isMaskON = maskon.floatValue > 0 ;
        isMaskON = EditorGUILayout.Toggle("是否启用遮罩", isMaskON);
        if(isMaskON)
        {
            material.SetFloat("_Mask_ON",1);
            material.EnableKeyword("_MASK_ON");
            m_MaterialEditor.TexturePropertySingleLine(new GUIContent("遮罩贴图"), maskTex_Sampler);
            m_MaterialEditor.TextureScaleOffsetProperty(maskTex_Sampler);


            isMaskClamp = maskClamp.floatValue > 0;
            isMaskClamp = EditorGUILayout.Toggle("是否使用纹理阻断", isMaskClamp);
            if(isMaskClamp)
            {
                material.SetInt("_MaskClamp",1);
            }
            else
            {
                material.SetInt("_MaskClamp",0);
            }

            isMaskUV2 = maskuv2.floatValue > 0;
            isMaskUV2 = EditorGUILayout.Toggle("是否使用UV2", isMaskUV2);
            if(isMaskUV2)
            {
                material.SetInt("_MaskUV2",1);
            }
            else
            {
                material.SetInt("_MaskUV2",0);
            }
            
            m_MaterialEditor.ShaderProperty(maskTexAlphaChannel, "遮罩通道选择");
            GUILayout.Space(5);

            ismaskRota = maskRota.floatValue > 0 ;
            ismaskRota = EditorGUILayout.Toggle("是否启用遮罩旋转", ismaskRota);
            GUILayout.Space(5);
            if(ismaskRota)
            {
                material.SetInt("_MaskRota", 1);
                
                EditorGUILayout.HelpBox("开启遮罩旋转时将无法使用遮罩流动", MessageType.None);
                GUILayout.Space(5);

                m_MaterialEditor.ShaderProperty(maskRotation, "遮罩旋转角度");
                GUILayout.Space(5);

                m_MaterialEditor.ShaderProperty(maskRotaSpeed, "遮罩旋转速度");
                GUILayout.Space(5);
            }
            else
            {
                material.SetInt("_MaskRota", 0);


            }


            ismaskCustom = maskCustom.floatValue > 0 ;
            ismaskCustom = EditorGUILayout.Toggle("是否启用自定义曲线", ismaskCustom);
            GUILayout.Space(5);

            
            if (ismaskCustom)
            {
                material.SetInt("_MaskCustom", 1);
                EditorGUILayout.HelpBox("CustomData1.zw控制遮罩纹理UV偏移,如果使用将自定义数据中依次添加UV2，Custom1.xyzw,Custom2.xyzw", MessageType.None);
                GUILayout.Space(5);
                
            }
            else
            {
                material.SetInt("_MaskCustom", 0);
                m_MaterialEditor.ShaderProperty(maskSpeedU, "U流动速度");
                GUILayout.Space(5);

                m_MaterialEditor.ShaderProperty(maskSpeedV, "V流动速度");
                GUILayout.Space(5);

            }

            
        }
        else
        {
            material.SetFloat("_Mask_ON",0);
            material.DisableKeyword("_MASK_ON");
            GUILayout.Space(5);
        }


 
    }


    public void GUI_Noisetexture(Material material) 
    {
        isNoiseON = noiseon.floatValue >0;//当前宏开启情况判断
        isNoiseON = EditorGUILayout.Toggle("是否启用扰动", isNoiseON);//创建开关
        if (isNoiseON)
        {
            material.SetFloat("_NOISE_ON",1);//宏赋值
            material.EnableKeyword("_NOISE_ON");//开宏
            m_MaterialEditor.TexturePropertySingleLine(new GUIContent("扰动贴图"), Noise_Sampler);//贴图位于赋值
            m_MaterialEditor.TextureScaleOffsetProperty(Noise_Sampler);//贴图UV

            isNoiseUV2 = noiseuv2.floatValue > 0;
            isNoiseUV2 = EditorGUILayout.Toggle("是否使用UV2", isNoiseUV2);
            if(isNoiseUV2)
            {
                material.SetInt("_NoiseUV2",1);
            }
            else
            {
                material.SetInt("_NoiseUV2",0);
            }

            m_MaterialEditor.ShaderProperty(noiseSpeedU, "U流动速度");//UV流动速度
            GUILayout.Space(5);//行间隔

            m_MaterialEditor.ShaderProperty(noiseSpeedV, "V流动速度");
            GUILayout.Space(5);

            isNoiseCustom = noiseCustom.floatValue >0;//判断自定义是否开启
            isNoiseCustom = EditorGUILayout.Toggle("是否启用自定义曲线", isNoiseCustom);//创建自定义曲线开关
            GUILayout.Space(5);


            if (isNoiseCustom)
            {
                material.SetInt("_NoiseCustom", 1);//开启赋值
                EditorGUILayout.HelpBox("CustomData2.x控制扰动强度,原有扰动强度将失效被隐藏，如果使用将自定义数据中依次添加UV2，Custom1.xyzw,Custom2.xyzw", MessageType.None);//如果开启的话会有文字提示
                
            }
            else
            {
                material.SetInt("_NoiseCustom", 0);
                m_MaterialEditor.ShaderProperty(NoiseInt, "扰动强度");//如果不开启使用扰动强度滑杆
                GUILayout.Space(5);
            }

            m_MaterialEditor.TexturePropertySingleLine(new GUIContent("扰动遮罩贴图"), noisemaskTex);//贴图位于赋值
            m_MaterialEditor.TextureScaleOffsetProperty(noisemaskTex);//贴图UV
            GUILayout.Space(5);
            GUILayout.Space(5);

        }
        else
        {
            material.SetFloat("_NOISE_ON",0);//宏赋值
            material.DisableKeyword("_NOISE_ON");//关闭宏
            GUILayout.Space(5);
        }
    }

    
    public void GUI_Dissolve(Material material) 
    {
            
            isDissolveON = Dissolveon.floatValue > 0 ;
            isDissolveON = EditorGUILayout.Toggle("是否启用溶解", isDissolveON);

            if (isDissolveON)
            {
                material.SetFloat("_DissolveMode",1);
                material.EnableKeyword("_DISSOLVE_SOFT");
                EditorGUILayout.HelpBox("当前溶解计算可以实现硬边溶解、软边溶解、光边溶解、异色宽边软溶解、定向溶解和Flowmap溶解，善用边缘宽度和混合强度可以更好实现不同效果。****如果出现溶解异常请检查溶解图通道选择", MessageType.None);
                m_MaterialEditor.TexturePropertySingleLine(new GUIContent("溶解贴图"), DissTex_Sampler);
                m_MaterialEditor.TextureScaleOffsetProperty(DissTex_Sampler);


                isDissolveUV2 = dissolveuv2.floatValue > 0;
                isDissolveUV2 = EditorGUILayout.Toggle("是否使用UV2", isDissolveUV2);
                if(isDissolveUV2)
                {
                    material.SetInt("_DissolveUV2",1);
                }
                else
                {
                    material.SetInt("_DissolveUV2",0);
                }
                
                m_MaterialEditor.ShaderProperty(DissolveTexAplha, "溶解通道选择");
                GUILayout.Space(5); 

                m_MaterialEditor.ShaderProperty(DissolveColor, "溶解颜色");
                GUILayout.Space(5);
                
                m_MaterialEditor.ShaderProperty(OutLineInt, "边缘宽度");
                GUILayout.Space(5);

                m_MaterialEditor.ShaderProperty(BlendInt, "混合强度");
                GUILayout.Space(5);

                isDissRota = dissRota.floatValue > 0 ;
                isDissRota = EditorGUILayout.Toggle("是否启用纹理旋转", isDissRota);
                GUILayout.Space(5);
                if(isDissRota)
                {
                    material.SetInt("_DissRota", 1);
                    
                    EditorGUILayout.HelpBox("开启纹理旋转时将无法使用纹理流动", MessageType.None);
                    GUILayout.Space(5);

                    m_MaterialEditor.ShaderProperty(dissRotation, "纹理旋转角度");
                    GUILayout.Space(5);

                }
                else
                {
                    material.SetInt("_DissRota", 0);
                    m_MaterialEditor.ShaderProperty(DissolveSpeedU, "U流动速度");
                    GUILayout.Space(5);

                    m_MaterialEditor.ShaderProperty(DissolveSpeedV, "V流动速度");
                    GUILayout.Space(5);
                }
            
                m_MaterialEditor.TexturePropertySingleLine(new GUIContent("溶解方向贴图"), DissDirTex);
                m_MaterialEditor.TextureScaleOffsetProperty(DissDirTex);
                GUILayout.Space(5);
                m_MaterialEditor.ShaderProperty(DissDirIntensity, "溶解方向叠加强度");
                GUILayout.Space(5);
                
                isDissCustom = DissolveCustom.floatValue > 0;
                isDissCustom = EditorGUILayout.Toggle("是否启用自定义曲线", isDissCustom);
                if(isDissCustom)
                {
                    material.SetInt("_DissolveCustom",1);
                    EditorGUILayout.HelpBox("CustomData2.y控制溶解进程,如果使用自定义曲线请保证溶解进程为0",MessageType.None);
                    GUILayout.Space(5);
                }
                else
                {
                    material.SetInt("_DissolveCustom",0);
                }






                m_MaterialEditor.ShaderProperty(DissolveAmount,"溶解进程");
                GUILayout.Space(5);
                
            }
            else
            {
                material.SetFloat("_DissolveMode",0);//宏赋值
                material.DisableKeyword("_DISSOLVE_SOFT");//关闭宏
                GUILayout.Space(5);
            }
               
 
    }
}
