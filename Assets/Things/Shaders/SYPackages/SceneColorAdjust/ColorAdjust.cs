using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Things.Shaders.SYPackages.SceneColorAdjust
{
    [System.Serializable,VolumeComponentMenu("JiZhan/ColorAdjust")]
    public class ColorAdjust : VolumeComponent, IPostProcessComponent
    {
        public FloatParameter sharpenIntensity = new ClampedFloatParameter(0f, 0f, 25f);

        public FloatParameter tonemapExposurePre = new FloatParameter(0.9f);

        public FloatParameter tonemapBrightnessPost = new FloatParameter(1.5f);

        public ClampedFloatParameter saturate = new ClampedFloatParameter(1f, -2f, 3f);

        public ClampedFloatParameter brightness = new ClampedFloatParameter(1f, 0f, 2f);

        public ClampedFloatParameter contrast = new ClampedFloatParameter(1f, 0.5f, 1.5f);

        public ClampedFloatParameter daltonize = new ClampedFloatParameter(0f, 0f, 2f);

        public ClampedFloatParameter sepia = new ClampedFloatParameter(0f, 0f, 1f);

        public ColorParameter tintColor = new ColorParameter(new Color(1, 1, 1, 0));

        public bool IsActive()
        {
            return    sharpenIntensity.overrideState
                   || tonemapExposurePre.overrideState
                   || tonemapBrightnessPost.overrideState
                   || saturate.overrideState
                   || brightness.overrideState
                   || contrast.overrideState
                   || daltonize.overrideState
                   || sepia.overrideState
                   || tintColor.overrideState
                   ;
        }

        public bool IsTileCompatible() => false;
    }   
}