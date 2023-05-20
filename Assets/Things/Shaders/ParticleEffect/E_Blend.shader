Shader "ShiYue/ParticleEffect/Blend"
{
    Properties {
		[HDR]_Color	("主颜色", Color) = (0.5,0.5,0.5,0.5)
		_MainTex	("主帖图", 2D)    = "white" {}
    }
	SubShader {
		Tags { "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" "Queue" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		ZTest LEqual
		Cull Off
 
		HLSLINCLUDE
			#pragma multi_compile_instancing 
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			
			UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
				UNITY_DEFINE_INSTANCED_PROP(half4, _Color)
				UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
			UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

			#define color UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Color)
			#define mainTex_ST UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _MainTex_ST)
		
            TEXTURE2D (_MainTex);	SAMPLER(sampler_MainTex);
		ENDHLSL
 
		Pass {
			Name "Normal"
			Tags 
			{ 
				"LightMode"="UniversalForward"
				"Queue" = "Transparent" 
			}
 
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
 
			struct Attributes {
				float4 positionOS	: POSITION;
				float2 uv0			: TEXCOORD0;
				half4  vertexColor	: COLOR;
			
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
 
			struct Varyings {
				float4 positionCS 	: SV_POSITION;
				float2 uv			: TEXCOORD0;
            	half4  vertexColor	: TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			Varyings vert(Attributes IN) {
				
				Varyings OUT = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID(IN);
    			UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
    			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				
				VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);

				OUT.positionCS	= positionInputs.positionCS;
				OUT.uv			= IN.uv0 * mainTex_ST.xy + mainTex_ST.zw;
				OUT.vertexColor = IN.vertexColor;

				return OUT;
			}
			
			half4 frag(Varyings IN) : SV_Target {

				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
				
				half3 finalColor = 1;
				finalColor *= albedo.rgb;
				finalColor *= IN.vertexColor.rgb;
				finalColor *= color.rgb;

				half finalAlpha = 1;
				finalAlpha *= albedo.a;
				finalAlpha *= IN.vertexColor.a;
				finalAlpha *= color.a;
				
				return half4(finalColor, finalAlpha);
			}
			ENDHLSL
		}
	}
}
