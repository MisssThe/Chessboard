#ifndef LIB
#define LIB

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

half4 SampleAlbedoAlpha(float2 uv, TEXTURE2D_PARAM(albedoAlphaMap, sampler_albedoAlphaMap))
{
    return SAMPLE_TEXTURE2D(albedoAlphaMap, sampler_albedoAlphaMap, uv);
}

half Alpha(half albedoAlpha, half4 color, half cutoff)
{
    half alpha = albedoAlpha * color.a;

    #if defined(_ALPHATEST_ON)
    clip(alpha - cutoff);
    #endif

    return alpha;
}

real3 ACESToneMapping(real3 color, real lum)
{
    const real A = 2.51f;
    const real B = 0.03f;
    const real C = 2.43f;
    const real D = 0.59f;
    const real E = 0.14f;

    color *= lum;
    return (color * (A * color + B)) / (color * (C * color + D) + E);
}

#endif
