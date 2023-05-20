Shader "ShiYue/Character/NPRChartacter"
{
	Properties
	{
		
		[CustomGroup] _ShadeSetting("贴图与阴影",float) = 1.0 
		[CustomTexture(_ShadeSetting)] _MainTex ("固有色贴图", 2D) = "white" {}
		[CustomColor(_ShadeSetting)]   _Color("_TintColor",Color)=(1,1,1,1)
		[CustomHeader(_ShadeSetting)] _SSSMapTip("RGB:阴影颜色  A:镭射遮罩", Float) = 0
		[CustomTexture(_ShadeSetting,_SSSSHADEMAP)] _SSSMap("阴影贴图",2D)="white" {}
		[CustomObject(_ShadeSetting)]  _SSSColor("SSS阴影颜色",Color)= (1,1,1,1)
		[CustomObject(_ShadeSetting)]  _SSSUseColorOrSSSMap("ColorOrSSSMap",Range(0,1))=0
		
		[CustomGroup] _MaskSetting("Mask贴图设置",float) = 1.0 
		[CustomHeader(_MaskSetting)] _MaskTip("R:高光遮罩  G:阴影位置偏移  B:粗糙度  A:自发光", Float) = 0
		[CustomTexture(_MaskSetting)]_MaskTex("Mask贴图",2D)="white" {}
		[CustomObject(_MaskSetting)]  _Roughness("粗糙强度",Range(0,1))= 1
		
		[CustomGroup] _LightSetting("灯光设置",float) = 1.0 
		[CustomKeywordEnum(_LightSetting, FAKELIGHT, DIRECTIONALLIGHT)] _LightMode("灯光模式",float) = 0.0 
		[CustomObject(_LightSetting)][HDR] _FakeLightColor ("灯光颜色(FakeLight模式下有用)", Color) = (1,1,1,1)
		[CustomLightDir(_LightSetting)] _FakeLightDir ("灯光方向(FakeLight模式下有用)", vector) = (0.5,0.5,0.5,1)
		
		[CustomGroup] _ShadeDataInfo("阴影数据配置",float) = 1.0
		[CustomColor(_ShadeDataInfo)]   _1stDarkColor("阴影颜色",Color)=(1,1,1,1)
		[CustomObject(_ShadeDataInfo)]  _1stShadow_Step("1st阴影阈值偏移",Range(0,1))= 0.5
		[CustomObject(_ShadeDataInfo)]  _1stShadow_Feather("1st阴影过度(硬->软)",Range(0,1))=0
		[CustomColor(_ShadeDataInfo)]   _2stDarkColor("内勾边颜色",Color)=(1,1,1,1)
		
		[CustomGroup] _EmissionSetting("自发光设置",float) = 1.0 
		[CustomObject(_EmissionSetting)][HDR] _EmissionColor ("颜色",Color)= (1,1,1,1)
		[CustomObject(_EmissionSetting)]  _UseEmissionColorOrBaseMap("UseColorOrBaseMap",Range(0,1))= 0
		[CustomObject(_EmissionSetting)] _Emission_Multiplier ("强度",Range(0,10))= 0.0
		
		[CustomGroup] _HightLightSetting("双层高光设置",float) = 1.0 
		[CustomHeader(_HightLightSetting)] _SpecularTip("亮层高光", Float) = 0
		[CustomObject(_HightLightSetting)][HDR]  _HightLightColor("高光颜色", Color) = (1,1,1,1)
		[CustomObject(_HightLightSetting)]  _HightLight_Instensity("高光强度", Range(0,10)) = 2.5
		[CustomObject(_HightLightSetting)]  _UseHightLightColorOrBaseMap("高光颜色混合固有色强度", Range(0, 1))= 0.6
		[CustomHeader(_HightLightSetting)] _NdVSpecularTip("暗层高光", Float) = 0
		[CustomObject(_HightLightSetting)]  _NdVSpecularWeight("暗层高光混合强度", Range(0.02, 1)) = 0.2
		[CustomObject(_HightLightSetting)]  _NdVSpecularInstensity("暗层高光强度", Range(0, 1)) = 0.38
		
		[CustomGroup] _IridencentSetting("镭射设置",float) = 2.0
        [CustomToggle(_IridencentSetting, _IRIDENCENT)] _IridencentOn ("开关", Float) = 0
		[CustomHeader(_IridencentSetting)] _IridesceneMapTip("使用“阴影贴图A通道”作为镭射遮罩", Float) = 0
        [CustomTexture(_IridencentSetting._IRIDENCENT)]_IridesceneMap("镭射颜色贴图", 2D)= "white" {}
		[CustomObject(_IridencentSetting._IRIDENCENT)] _IridesceneBlendAlbedo("固有色混合镭射颜色强度", Range(0, 1)) = 0.5
		[CustomObject(_IridencentSetting._IRIDENCENT)] _IridesceneDiffuseIntensity("固有色镭射亮度", Range(0, 8)) = 1
		[CustomObject(_IridencentSetting._IRIDENCENT)] _IridesceneMapIntensity("高光镭射亮度", Range(0, 8)) = 1
		
		[CustomGroup] _RimLightSetting("边缘光设置",float) = 1.0 
		[CustomLightDir(_RimLightSetting)]   _RimLightDir("边缘光方向", vector) = (0.5,0.5,0.5,1)
		[CustomObject(_RimLightSetting)]  _RimLightColor("颜色",Color)=(1,1,1,1)
		[CustomObject(_RimLightSetting)]  _UseRimColorOrBaseMap("UseColorOrBaseMap",Range(0,1)) = 0
		[CustomObject(_RimLightSetting)]  _RimLightThreshold("阈值",Range(0,1))= 0
		[CustomObject(_RimLightSetting)]  _RimLightMin("过渡",Range(0,1))= 0
		[CustomObject(_RimLightSetting)]  _RimLightInstensity("整体强度",Range(0,100))= 0
		
		[CustomGroup] _OutlineSetting("描边设置",float) = 1.0 
		[CustomKeywordEnum(_OutlineSetting, _NORMAL, _COLOR, _UV)] _OutlineSource ("平滑法线来源：", Float) = 0
		[CustomHeader(_OutlineSetting)] _VertexColorTip("描边颜色默认使用模型顶点色", Float) = 0
		[CustomToggle(_OutlineSetting)] _WhiteColorUseOutlineColor ("白色顶点色区域使用描边颜色", Float) = 1
		[CustomHeader(_OutlineSetting)] _UseVertexColorTip("当“白色顶点色区域使用描边颜色”功能关闭时“描边颜色混合模型顶点色”功能才生效", Float) = 0
		[CustomToggle(_OutlineSetting)] _UseVertexColor ("模型顶点色混合描边颜色", Float) = 0
		[CustomObject(_OutlineSetting)][HDR]_OutlineColor("描边颜色", Color) =(0,0,0,0)
		[CustomObject(_OutlineSetting)]_Outline_Width ("描边宽度", Range(0,10)) = 1
		
		[CustomGroup] _DissolveSetting("溶解设置（特效）",float) = 1.0 
		[CustomToggle(_DissolveSetting, _DISSOLVE)] _DissolveOn ("开关", Float) = 0
		[CustomHeader(_DissolveSetting._DISSOLVE)] _DissolveTip("只对半透材质有效", Float) = 0
    	[CustomTexture(_DissolveSetting._DISSOLVE)] _DissolveTex("溶解贴图", 2D) = "white"{}
    	[CustomObject(_DissolveSetting._DISSOLVE)] _dissolveIntensity("溶解强度", Range(-1, 1)) = 0
		[CustomObject(_DissolveSetting._DISSOLVE)] _dissolveSoftnessIntensity("溶解边缘柔软度", Range(0, 1)) = 0
    	[CustomObject(_DissolveSetting._DISSOLVE)] [HDR]_dissolveBrightnessColor("溶解亮边颜色", Color) = (1, 1, 1, 1)
	    [CustomObject(_DissolveSetting._DISSOLVE)] _dissolveBrightnessWidth("溶解亮边宽度", Range(0, 1)) = 0.1
		
		[CustomGroup] _ReflectionSetting("倒影设置",float) = 1.0 
		[CustomObject(_ReflectionSetting)] _ReflectionColorIntensity ("倒影颜色强度", Range(0,1)) = 0.8
		[CustomObject(_ReflectionSetting)] _ReflectionIntensity ("倒影强度", Range(0,8)) = 1
		[CustomObject(_ReflectionSetting)] _ReflectionRange ("倒影范围", Range(-1, 1)) = 1
		
		// Blending state
        [HideInInspector] _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
        [HideInInspector] _ZTest("__zt", Float) = 4.0
		
	}
	
	SubShader
	{
		LOD 100
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
		Pass
		{
			Tags {"LightMode"="UniversalForward"}
			Cull [_Cull]
			ZWrite [_ZWrite]
			ZTest [_ZTest]
			Blend SrcAlpha OneMinusSrcAlpha//[_SrcBlend] [_DstBlend]
			
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#pragma shader_feature BLEND_MODE_OPAQUE BLEND_MODE_CUTOUT BLEND_MODE_TRANSPARENT
			
			#pragma multi_compile_local_fragment FAKELIGHT DIRECTIONALLIGHT
			#pragma multi_compile_local_fragment _ _IRIDENCENT
			#pragma multi_compile_local_fragment _ _DISSOLVE
			
			#pragma multi_compile_instancing

			#include "..\..\HLSLIncludes\NPRCharacterInput.hlsl"
			#include "..\..\HLSLIncludes\NPRCharacterPass.hlsl"
			ENDHLSL
		}

		Pass {
	        Name "Outline"
			Tags { "LightMode" = "Outline" }

	        Cull Front
			ZWrite On
			ZTest LEqual
			Offset 1, 1
			//Offset [_Offset_Factor],[_Offset_Units]
	        HLSLPROGRAM
	        #pragma target 3.0
	        
			#pragma multi_compile_local_vertex _NORMAL _COLOR _UV
	        #pragma multi_compile_local_fragment _ _DISSOLVE
		
	        #pragma vertex OutlinePassVertNew
	        #pragma fragment OutlinePassFragmentNew

	        #define OUTLINE_WIDTH_NORAML 0.15f
	        #define OUTLINE_WIDTH_UV 0.35f

			#include "..\..\HLSLIncludes\NPRCharacterInput.hlsl"
	        #include "..\..\HLSLIncludes\NPRCharacterOutLinePass.hlsl"
	        
	        ENDHLSL
	    }
		
		Pass
		{
			Tags {"LightMode"="PlanarReflection"}
			Cull [_Cull]
			ZWrite [_ZWrite]
			ZTest [_ZTest]
			Blend SrcAlpha OneMinusSrcAlpha//[_SrcBlend] [_DstBlend]
			
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment ReflectionPassFragment
			#pragma target 3.0
			
			#pragma shader_feature BLEND_MODE_OPAQUE BLEND_MODE_CUTOUT BLEND_MODE_TRANSPARENT
			
			#pragma multi_compile_local_fragment FAKELIGHT DIRECTIONALLIGHT
			#pragma multi_compile_local_fragment _ _IRIDENCENT
			
			#pragma multi_compile_instancing

			#include "..\..\HLSLIncludes\NPRCharacterInput.hlsl"
			#include "..\..\HLSLIncludes\NPRCharacterReflectionPass.hlsl"
			ENDHLSL
		}
		
		Pass
		{
			Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            Blend One Zero
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull [_Cull]
			
			HLSLPROGRAM

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #pragma shader_feature_local_fragment _ALPHATEST_ON
			
			#include "..\..\HLSLIncludes\NPRCharacterInput.hlsl"
			#include "../../HLSLIncludes/CharacterShadowCasterPass.hlsl"

            ENDHLSL
		}

	}

	CustomEditor "CustomShaderEditor.CustomShaderGUINew"
}
