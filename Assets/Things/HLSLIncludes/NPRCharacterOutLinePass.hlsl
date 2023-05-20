#ifndef _NRPCHARACTEROUTLINEPASS
#define _NRPCHARACTEROUTLINEPASS

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "..\HLSLIncludes\NPRCharacterInput.hlsl"

struct OutLineAttributes
{
    float4 positionOS   : POSITION;
    float2 texcoord     : TEXCOORD0;
    float3 normalOS     : NORMAL;
    float4 color : COLOR;
    
    #if !_NORMAL
    float4 tangentOS : TANGENT;
    #endif
    
    #if _UV
    float4 uv1 : TEXCOORD1;
    #endif
};

struct OutLineVaryings
{
    float4 positionCS : SV_POSITION;
    float2 uv  : TEXCOORD0;
    float4 color : TEXCOORD1;
};

float3 OctahedronToUnitVector(float2 oct)
{
    // float3 unitVec = float3(oct, 1 - dot(float2(1, 1), abs(oct)));
    //
    // if (unitVec.z < 0)
    // {
    //     unitVec.xy = (1 - abs(unitVec.yx)) *(unitVec.xy >= 0 ? float2(1, 1) : float2(-1, -1));
    // }
    float3  unitVec = float3(oct.x, oct.y, 1.0f - abs(oct.x) -  abs(oct.y));
    float t = max( -unitVec.z, 0.0f );
    unitVec.x += unitVec.x >= 0.0f ? -t : t;
    unitVec.y += unitVec.y >= 0.0f ? -t : t;
    return normalize(unitVec);
}

float4 OutLinePositionCS(float3 normalOS, float4 positionOS, float outlineMask)
{
    
    #if _NORMAL
    
        float3 normalVS = TransformWorldToViewDir(TransformObjectToWorldNormal(normalOS), true);

        float4 positionVS = mul(UNITY_MATRIX_MV, positionOS);

        // 顶点往后偏移，防止挡住前面的顶点
        float3 normalizePos = normalize(positionVS.xyz);
        float3 offsetPositionVS = normalizePos * 0.1f + positionVS.xyz;
				    
        // A magic num?
        float widthUniformityCorrecting = positionVS.z * rcp(0.03f);
        widthUniformityCorrecting *= 66.6666718f;
        widthUniformityCorrecting = sqrt(-widthUniformityCorrecting);
        widthUniformityCorrecting *= 0.00027f;

        float2 finalPositionVSXY = normalVS.xy * widthUniformityCorrecting * OUTLINE_WIDTH_NORAML + offsetPositionVS.xy;
        float  finalPositionVSZ  = offsetPositionVS.z;
				    
        float4 finalPositionVS = float4(finalPositionVSXY.xy, finalPositionVSZ, positionVS.w);
        float4 positionCS = TransformWViewToHClip(finalPositionVS.xyz);
    
    #else
    
        float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, normalOS);
        normal.z = 0.001f;
        normal = normalize(normal);

        float3 positionVS = mul(GetWorldToViewMatrix(), float4(mul(GetObjectToWorldMatrix(), float4(positionOS.xyz, 1.0)).xyz, 1.0)).xyz;;

        float linewidth = -positionVS.z / (unity_CameraProjection[1].y);
        linewidth = sqrt(linewidth);
        linewidth *= 0.01;
        linewidth *= OUTLINE_WIDTH_UV;

        // positionVS.z += _Offset_Z * 0.001;
        positionVS.xy = normal.xy * linewidth * outlineMask + positionVS.xy;

        float4 positionCS = TransformWViewToHClip(positionVS.xyz);
    
    #endif

    return positionCS;
}

OutLineVaryings OutlinePassVertNew (OutLineAttributes input)
{
    OutLineVaryings output = (OutLineVaryings)0;

    float3 normal = input.normalOS;

    #if _COLOR
    normal = normalize(input.color.rgb * 2 - 1);

    float3 normalOS = input.normalOS;
    float4 tangentOSOS = input.tangentOS;
    float3 binormal = cross(normalize(normalOS), normalize(tangentOSOS.xyz)) * tangentOSOS.w;

    float3x3 objectToTangentMatrix = float3x3(tangentOSOS.xyz, binormal, normalOS);

    normal = normalize(mul(normal, objectToTangentMatrix));

    #elif _UV
    float3 normalTS = OctahedronToUnitVector(input.uv1.xy);
    normalTS = normalize(normalTS);
    float3 normalOS = normalize(input.normalOS);
    float3 tangentOS = normalize(input.tangentOS.xyz);
    float sign = input.tangentOS.w * GetOddNegativeScale();
    float3 bitangentOS = cross(normalOS, tangentOS) * sign;
    bitangentOS  = normalize(bitangentOS);
    float3x3 tangentToNormal = float3x3(input.tangentOS.xyz, bitangentOS,
                                       input.normalOS.xyz);
    normal = mul(normalTS, tangentToNormal);
    #endif
    
    output.positionCS = OutLinePositionCS(normal, input.positionOS, input.color.a);
    output.uv = input.texcoord;

    //_WhiteColorType
    
    half vertexColorLuminance = step(1, Luminance(input.color.rgb));
    
    output.color = (1 - vertexColorLuminance) * input.color + vertexColorLuminance * _OutlineColor;
    output.color = (1 - _WhiteColorUseOutlineColor) * input.color + output.color * _WhiteColorUseOutlineColor;
    
    half mask = 1 - _WhiteColorUseOutlineColor;
    mask *= _UseVertexColor;
    output.color.rgb *= (1 - mask) + mask * _OutlineColor.rgb;
    
    return output;
}

half4 OutlinePassFragmentNew (OutLineVaryings input) : SV_Target
{
    #if _DISSOLVE
    //===================溶解计算===================
    half dissolveValue = SAMPLE_TEXTURE2D(_DissolveTex, sampler_DissolveTex, input.uv * _DissolveTex_ST.xy + _DissolveTex_ST.zw).r;

    half dissolveIntensity = _dissolveIntensity;
    half clipValue = smoothstep(dissolveIntensity, dissolveIntensity + _dissolveSoftnessIntensity, dissolveValue);

    clip(clipValue - 0.001);
    #endif
    
    half4 finalOutlineColor;
    
    finalOutlineColor.rgb = input.color.rgb;
    finalOutlineColor.a = 1;
    
    return finalOutlineColor;
}

#endif
