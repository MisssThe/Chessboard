#ifndef CHARACTERSHADOWCASTERPASS
#define CHARACTERSHADOWCASTERPASS

#include "Lib.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

struct ShadowAttributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    half4  color        : COLOR;
    float2 texcoord     : TEXCOORD0;
};

struct ShadowVaryings
{
    float2 uv           : TEXCOORD0;
    float4 positionCS   : SV_POSITION;
};

float4 GetShadowPositionHClip(ShadowAttributes input)
{
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

    #if UNITY_REVERSED_Z
    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
    #else
    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
    #endif

    return positionCS;
}

ShadowVaryings ShadowPassVertex(ShadowAttributes input)
{
    ShadowVaryings output;

    #ifdef _RECEIVE_WIND
    float swingAlpha = input.positionOS.y * rcp(_ObjHeight);
    swingAlpha = smoothstep(_SwingHeight, 1.0f, swingAlpha) * input.color.g;
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    input.positionOS.xyz = SelfSwing(positionWS, float3(0,0,0), swingAlpha);
    #endif
    
    output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
    output.positionCS = GetShadowPositionHClip(input);
    return output;
}

half4 ShadowPassFragment(ShadowVaryings input) : SV_TARGET
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    
    Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_MainTex, sampler_MainTex)).a, _Color, _Cutoff);
    return 0;
}

#endif
