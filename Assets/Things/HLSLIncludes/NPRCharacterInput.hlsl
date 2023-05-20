#ifndef _NRPCHARACTERINPUT
#define _NRPCHARACTERINPUT

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

// #include "..\CGIncludes\CustomScreenSpaceShadow.hlsl"

CBUFFER_START(UnityPerMaterial)
float4 _MainTex_ST,//贴图与阴影
       _IridesceneMap_ST,//镭射设置
       _DissolveTex_ST;//溶解设置

float _Outline_Width;//描边设置

half4 _Color, _SSSColor,//贴图与阴影
      _FakeLightColor, _FakeLightDir,//灯光设置
      _1stDarkColor, _2stDarkColor,//阴影数据配置
      _EmissionColor, //自发光设置
      _HightLightColor, //双层高光设置
      _RimLightDir, _RimLightColor,//边缘光设置
      _OutlineColor,//描边设置
      _dissolveBrightnessColor;//溶解设置

half _SSSUseColorOrSSSMap,//贴图与阴影
     _Roughness,//Mask贴图设置
     _1stShadow_Step, _1stShadow_Feather,//阴影数据配置
     _UseEmissionColorOrBaseMap, _Emission_Multiplier,//自发光设置
     _HightLight_Instensity, _UseHightLightColorOrBaseMap, _NdVSpecularWeight, _NdVSpecularInstensity,//双层高光设置
     _IridesceneBlendAlbedo, _IridesceneDiffuseIntensity, _IridesceneMapIntensity,//镭射设置
     _UseRimColorOrBaseMap, _RimLightThreshold, _RimLightMin, _RimLightInstensity,//边缘光设置
     _WhiteColorUseOutlineColor, _UseVertexColor, //描边设置
     _dissolveIntensity, _dissolveSoftnessIntensity, _dissolveBrightnessWidth,//溶解设置
     _ReflectionColorIntensity, _ReflectionIntensity, _ReflectionRange,//反射设置
     _Cutoff;

CBUFFER_END
float3 _LightDirection;

TEXTURE2D(_MainTex  );    SAMPLER(sampler_MainTex  );    
TEXTURE2D(_SSSMap   );    SAMPLER(sampler_SSSMap   );     
TEXTURE2D(_MaskTex  );    SAMPLER(sampler_MaskTex  );    
// TEXTURE2D(_NormalTex);    SAMPLER(sampler_NormalTex);  
TEXTURE2D(_IridesceneMap);    SAMPLER(sampler_IridesceneMap);
TEXTURE2D(_DissolveTex);    SAMPLER(sampler_DissolveTex);

struct NPRBaseData
{
    half3 albedo;
    half  alpha;
    half4 sssMap;
    half4 maskMap;
    half3 emission;
    half3 normalTS;
};

struct CustomLight
{
    float3 dir;
    half3 color;
};

half4 MainTexSample(float2 uv)
{
    half4 mainTexCol=0;
    mainTexCol= SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
    return mainTexCol;
}

half4 NormalTexSample(float2 uv)
{
  half4 normalTexCol=half4(0.0, 0.0, 1.0,0);
  #if _NORMALMAP
  normalTexCol = SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, uv);
  #endif
  return normalTexCol;
}

half4 MaskTexSample(float2 uv)
{
    half4 maskTexCol=0;
    maskTexCol= SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, uv);
    return maskTexCol;
}

half4 SSSMapSample(float2 uv)
{
    half4 SSSMapCol=0;
    SSSMapCol= SAMPLE_TEXTURE2D(_SSSMap, sampler_SSSMap, uv);
    return SSSMapCol;
}

half Alpha(half4 albedoAlpha)
{
  half alpha = albedoAlpha.a;
  return alpha;
}
half3 EmissionSample(float2 uv)
{
  return 0;
}

inline void InitializeNPRLitSurfaceData(float2 uv, out NPRBaseData outNPRBaseData)
{
    outNPRBaseData = (NPRBaseData)0;

    half4 albedoAlpha = MainTexSample(uv);

    outNPRBaseData.albedo = albedoAlpha.rgb * _Color.rgb;

    outNPRBaseData.alpha = albedoAlpha.a * _Color.a;

    outNPRBaseData.sssMap = SSSMapSample(uv);

    #if BLEND_MODE_CUTOUT	
        clip(outNPRBaseData.alpha - _Cutoff);
	#endif

    half4 maskMap = MaskTexSample(uv);
    outNPRBaseData.maskMap = maskMap;

    half4 normalMap =  NormalTexSample(uv);
    outNPRBaseData.normalTS = normalMap.xyz;
    #if _NORMALMAP
    outNPRBaseData.normalTS = UnpackNormalScale(normalMap, 1);
    #endif
    outNPRBaseData.emission = EmissionSample(uv);
    

}

#endif