#ifndef _NRPCHARACTERPASS
#define _NRPCHARACTERPASS

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#include "..\HLSLIncludes\NPRCharacterInput.hlsl"

struct Attributes
{
	float4 positionOS   : POSITION;
	float3 normalOS     : NORMAL;
	float4 tangentOS    : TANGENT;
	float2 texcoord     : TEXCOORD0;
	float2 lightmapUV   : TEXCOORD1;
};

struct Varyings
{
	float4 positionCS                   : SV_POSITION;
	float2 uv                           : TEXCOORD0;
	float3 positionWS                   : TEXCOORD1;
	DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 2);
	#if defined(_NORMALMAP)
	float4 normalWS                     : TEXCOORD3; // xyz: normal, w: viewDir.x
	float4 tangentWS                    : TEXCOORD4; // xyz: tangent, w: viewDir.y
	float4 bitangentWS                  : TEXCOORD5; // xyz: bitangent, w: viewDir.z
	#else
	float3 normalWS                     : TEXCOORD3;
	float3 viewDirWS                    : TEXCOORD4;
	#endif
    
	#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
	float4 shadowCoord                  : TEXCOORD5;
	#endif
};

Varyings VertexNormal(Attributes input) 
{
	Varyings v = (Varyings)0;

	VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
	
	float3 positionWS = positionInputs.positionWS;
	float3 positionVS = positionInputs.positionVS;
	float4 positionCS = positionInputs.positionCS;

	v.positionCS = positionCS;
	v.positionWS = positionWS;
	v.uv.xy = TRANSFORM_TEX(input.texcoord, _MainTex);

	return v;
}

half4 fragNormal (Varyings input) : SV_Target
{
	half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv.xy);
	return color;
}

Varyings vert(Attributes input)
{
    Varyings output = (Varyings)0;

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

	output.positionCS = vertexInput.positionCS;

    half3 viewDirWS = normalize(_WorldSpaceCameraPos.xyz - vertexInput.positionWS);
	
    output.uv.xy = TRANSFORM_TEX(input.texcoord, _MainTex);

#ifdef _NORMALMAP
    output.normalWS = half4(normalInput.normalWS, viewDirWS.x);
    output.tangentWS = half4(normalInput.tangentWS, viewDirWS.y);
    output.bitangentWS = half4(normalInput.bitangentWS, viewDirWS.z);
#else
    output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);
    output.viewDirWS = viewDirWS;
#endif

	OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
	OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

    output.positionWS = vertexInput.positionWS;


    return output;
}

inline void MainLightSetup(InputData inputdata,  out CustomLight light)
{
	#ifdef DIRECTIONALLIGHT
	light.dir = _MainLightPosition.xyz;
	light.color = _MainLightColor.rgb;
	#else
	light.dir=normalize(_FakeLightDir.xyz);
	light.color=_FakeLightColor.rgb;
	#endif
}

void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData)
{
    inputData = (InputData)0;
	inputData.positionWS = input.positionWS;

#ifdef _NORMALMAP
    half3 viewDirWS = half3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
    inputData.normalWS = TransformTangentToWorld(normalTS,
    half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
#else
    half3 viewDirWS = input.viewDirWS;
    inputData.normalWS = input.normalWS;
#endif

	inputData.normalWS = normalize(inputData.normalWS);
    inputData.viewDirectionWS = SafeNormalize(viewDirWS);
    inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
	inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
}

half LinearStep(half minValue, half maxValue, half In)
{
    return saturate((In-minValue) / (maxValue - minValue));
}

 half4 NPRShading(NPRBaseData nprbaseData, InputData inputData, float2 uv)
 {
	//===================基础数据准备===================
	half3 albedo = nprbaseData.albedo;
	half3 rampCol = nprbaseData.sssMap.rgb;
	half iridesceneMask = nprbaseData.sssMap.a;
	half nlmask = nprbaseData.maskMap.g;
	half specularMask = nprbaseData.maskMap.x;

	//===================光照准备===================
	CustomLight light;
	MainLightSetup(inputData, light);

	half NdotL = dot(inputData.normalWS, light.dir);
	half halflambert = NdotL * 0.5 + 0.5;
	half3 halfDir = normalize(inputData.viewDirectionWS+light.dir);
	half NdotH = saturate(dot(inputData.normalWS, halfDir));
	half LdotH = saturate(dot(light.dir, halfDir));
	half NdotV = dot(inputData.viewDirectionWS, inputData.normalWS);

	//混合高光计算
	half mixedSpecularWeight = NdotH - saturate(NdotV);
	half mask = ceil(saturate(mixedSpecularWeight));
	mixedSpecularWeight = mixedSpecularWeight * 0.5 + 0.5;
	mixedSpecularWeight = lerp(0.5, mixedSpecularWeight, _NdVSpecularWeight * 50);
	mixedSpecularWeight = saturate(mixedSpecularWeight);
	NdotH = lerp(NdotV, NdotH, mask);
	
	//===================镭射计算===================
	half3 iridesceneValue = 1;

	#if _IRIDENCENT
	half iridesceneMapUV = NdotV * _IridesceneMap_ST.x + _IridesceneMap_ST.z;
	iridesceneValue = SAMPLE_TEXTURE2D(_IridesceneMap, sampler_IridesceneMap, half2(iridesceneMapUV, 0.5)).rgb;
	
	albedo *= lerp(half3(1, 1, 1), iridesceneValue * _IridesceneDiffuseIntensity, _IridesceneBlendAlbedo * iridesceneMask);

	iridesceneValue *= _IridesceneMapIntensity;
	iridesceneValue = lerp(half3(1, 1, 1), iridesceneValue, iridesceneMask);

	#endif
	//===================阴影颜色计算===================
		//阴影位置偏移计算
		half maskstep = floor((nlmask - 0.5)+1);
		//以灰度值0.5为原点，少于0.5的使用公式1.2 *x -.0.1 高于 0.5 使用 1.25 * (x-0.5) - 0.12,一般超过0.5，我们可以认为这个区域逐渐趋于常亮的 或者基本常亮
	
		float2 nlmaskoffset = nlmask * float2(1.2, 1.25) - float2(0.1, 0.12);
	
		half offset = lerp(nlmaskoffset.x, nlmaskoffset.y, maskstep);
	
		//阴影颜色计算
		half3 xGlobalCharacterTwoShadowColor = half3(0.7869117,0.76476,0.9044118);
	
		half3 shadowCol = lerp(_SSSColor.rgb * albedo, rampCol, _SSSUseColorOrSSSMap);
			
		half3 _1stshadowCol = shadowCol * _1stDarkColor.rgb;

		half3 _2stshadowCol = shadowCol * _2stDarkColor.rgb;
		_2stshadowCol *= xGlobalCharacterTwoShadowColor;
	
		half _1stnlmask = 1 - LinearStep(_1stShadow_Step,_1stShadow_Step + _1stShadow_Feather,0.5 * (halflambert + offset));
		
		half3 bright_2_1stshadowCol = lerp(albedo, _1stshadowCol, _1stnlmask);
	

		//用第二次范围更小的阴影颜色充当内描边
		half _2stnlmask = 1 - floor(nlmask + 0.9);
	
		half3 diffuseCol = lerp(bright_2_1stshadowCol, _2stshadowCol, _2stnlmask);

	
	//===================PBR高光计算===================
		//PBR数据准备
		half perceptualRoughness = max(nprbaseData.maskMap.z * _Roughness, 1e-5);
		// half perceptualRoughness = min(1, max(nprbaseData.maskMap.z, 1e-5)) * _Roughness;//2023.3.24修改为GGX计算
	
		half roughness = perceptualRoughness * perceptualRoughness;
		half roughness2 = roughness * roughness;
		half normalizationTerm = roughness * 4.0h + 2.0h;
		half roughness2MinusOne = roughness2 - 1.0h;

		//GGX
		float d = NdotH * NdotH * roughness2MinusOne + 1.00001f;
		half LoH2 = LdotH * LdotH;
	
		half specularTerm = roughness2 / ((d * d) * max(0.1h, LoH2) * normalizationTerm);
		// half specularTerm = roughness2 / (PI * (d * d));//2023.3.24修改为GGX计算
	
		//高光形状
		specularMask *= saturate(1 - _1stnlmask);
		specularTerm = saturate(specularTerm);

		//高光颜色
		half3 specularCol = lerp(_HightLightColor.rgb, albedo, _UseHightLightColorOrBaseMap);
		specularCol *= specularTerm * lerp(_NdVSpecularInstensity * _HightLight_Instensity, _HightLight_Instensity, mixedSpecularWeight);
		specularCol *= specularMask;
		specularCol *= iridesceneValue;
	
		// specularTerm *= saturate(1-_1stnlmask);
		//
		// half hardspecularTerm = saturate(floor(1 + specularTerm - _HightLight_Threshold));
		// specularTerm = lerp(specularTerm, hardspecularTerm, _HightLight_Feather);
		// specularTerm = max(specularTerm, 1e-5);
		// specularTerm *= specularMask;
		// half3 specularCol = specularTerm.xxx * _HightLight_Instensity;
		// specularCol *= _HightLightColor.rgb;
		// specularCol *= iridesceneValue;

	//===================边缘光计算===================
	half NdotRimL = saturate(dot(inputData.normalWS, _RimLightDir.xyz));
	half inverseNdoV = 1 - saturate(NdotV);
	half rimvalue = LinearStep(_RimLightThreshold - _RimLightMin,_RimLightThreshold + _RimLightMin,inverseNdoV);

	rimvalue *= _RimLightInstensity;
	rimvalue *= NdotRimL;

	rimvalue  = saturate(rimvalue);
	half3 rimColor = rimvalue * _RimLightColor.xyz;

	//===================自发光计算===================
	half3 emissionCol = lerp(_EmissionColor.rgb, albedo, _UseEmissionColorOrBaseMap);
	emissionCol *= _Emission_Multiplier;
	emissionCol *= nprbaseData.maskMap.w;
	emissionCol *= (1 - _2stnlmask);

	//===================最终颜色计算===================
	half3 finalCol = diffuseCol + specularCol;
	finalCol *= light.color.xyz;
	finalCol += rimColor;	
	finalCol += emissionCol;

	half alpha = nprbaseData.alpha;

	//===================溶解计算===================
	#if _DISSOLVE
	half dissolveValue = SAMPLE_TEXTURE2D(_DissolveTex, sampler_DissolveTex, uv.xy * _DissolveTex_ST.xy + _DissolveTex_ST.zw).r;

	half dissolveIntensity = _dissolveIntensity;
	half clipValue = smoothstep(dissolveIntensity, dissolveIntensity + _dissolveSoftnessIntensity, dissolveValue);

	// clip(clipValue - 0.001);
            	
	half dissolveBrightnessArea = smoothstep(dissolveIntensity + _dissolveBrightnessWidth, dissolveIntensity + _dissolveSoftnessIntensity * ceil(_dissolveBrightnessWidth) + _dissolveBrightnessWidth, dissolveValue);

	finalCol = lerp(_dissolveBrightnessColor.rgb, finalCol, dissolveBrightnessArea);
	alpha = saturate(alpha * clipValue);
	#endif
	
	return half4(finalCol, alpha);
 }

half4 frag (Varyings input) : SV_Target
{
	NPRBaseData nprbaseData;
    InitializeNPRLitSurfaceData(input.uv.xy, nprbaseData);

	InputData inputData;
    InitializeInputData(input, nprbaseData.normalTS, inputData);
	
	half4 color = NPRShading(nprbaseData, inputData, input.uv);
	
	return color;

}


#endif