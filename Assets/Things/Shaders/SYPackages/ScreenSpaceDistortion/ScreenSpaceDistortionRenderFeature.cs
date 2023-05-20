using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SYPackages.PostProcessing.ScreenSpaceDistortion
{
    public class ScreenSpaceDistortionRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class DistortionSetting
        {
            public LayerMask mask = -1; //EveryThing
            //最好能在渲染场景之前绘制，这样可以避免触发ResolveAA
            public RenderPassEvent passEvent = RenderPassEvent.BeforeRenderingOpaques;
            public bool reUseOpaqueDepth = false;
            public DownSamplingMode downSamplingMode = DownSamplingMode.Half;
        }

        public enum DownSamplingMode
        {
            Normal = 0,
            Half = 1,
            Quarter = 2,
        }

        class ScreenSpaceDistortionRenderPass : ScriptableRenderPass
        {
            private const string m_ProfilerTag = "Draw Distortion Texture";
            internal static readonly string DistortionTextureName = "_CameraDistortionTexture";
            internal static readonly int DistortionTexturePropertyID = Shader.PropertyToID("_CameraDistortionTexture");


            ProfilingSampler m_ProfilingSampler = new ProfilingSampler(m_ProfilerTag);
            internal List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();

            RenderTargetHandle m_DistortionTexture;
            //RenderTexture m_DistortionTexture;

            private DistortionSetting setting;

            //private RenderTargetIdentifier sourceRTColor;
            private RenderTargetIdentifier sourceRTDepth;


            public ScreenSpaceDistortionRenderPass(DistortionSetting setting)
            {
                this.setting = setting;
                // Configures where the render pass should be injected.
                renderPassEvent = setting.passEvent;
                m_DistortionTexture.Init(DistortionTextureName);
                m_ShaderTagIdList.Add(new ShaderTagId("Distortion"));
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                this.sourceRTDepth = renderingData.cameraData.renderer.cameraDepthTarget;
                // ForwardRenderer.CopyDepthBuffer(ref sourceRTDepth);
            }

            // This method is called before executing the render pass.
            // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
            // When empty this render pass will render to the active camera render target.
            // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
            // The render pipeline will ensure target Setup and clearing happens in an performance manner.
            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                var rtdesc = cameraTextureDescriptor;
                rtdesc.colorFormat = RenderTextureFormat.ARGB32;
                rtdesc.useMipMap = false;
                rtdesc.msaaSamples = 1;
                rtdesc.depthBufferBits = 0;
                rtdesc.sRGB = false;
                rtdesc.useMipMap = false;

                if (!setting.reUseOpaqueDepth)
                {
                    rtdesc.width = rtdesc.width >> (int) setting.downSamplingMode;
                    rtdesc.height = rtdesc.height >> (int) setting.downSamplingMode;
                }

                cmd.GetTemporaryRT(m_DistortionTexture.id, rtdesc);

                if (setting.reUseOpaqueDepth)
                {
                    ConfigureTarget(m_DistortionTexture.id, sourceRTDepth);
                }
                else
                {
                    ConfigureTarget(m_DistortionTexture.id, m_DistortionTexture.id); //不画深度
                }

                ConfigureClear(ClearFlag.Color, new Color(0.498f, 0.498f, 0, 1)); //我们会对uv扭曲贴图进行 压缩操作，所以初始颜色会不太一样
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                DrawingSettings drawingSettings =
                    CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, SortingCriteria.CommonTransparent);

                FilteringSettings
                    filteringSetting =
                        new FilteringSettings(RenderQueueRange.all, (int) setting.mask); //, -1, renderingLayerMask);

                var cmd = CommandBufferPool.Get(m_ProfilerTag);

                using (new ProfilingScope(cmd, m_ProfilingSampler))
                {
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                    context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSetting);
                    cmd.SetGlobalTexture(DistortionTexturePropertyID, m_DistortionTexture.id);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                if (cmd == null)
                    throw new ArgumentNullException("cmd");

                if (m_DistortionTexture != RenderTargetHandle.CameraTarget)
                {
                    cmd.ReleaseTemporaryRT(m_DistortionTexture.id);
                }
            }
        }

        public DistortionSetting setting = new DistortionSetting();
        private ScreenSpaceDistortionRenderPass m_ScriptablePass;

        public override void Create()
        {
            if (isActive)
            {
                if (!this.setting.reUseOpaqueDepth) 
                {
                    //这里写死优化？
                    this.setting.passEvent = RenderPassEvent.BeforeRenderingOpaques;
                }
                m_ScriptablePass = new ScreenSpaceDistortionRenderPass(this.setting);
            }
            //RenderingUtils.UseScreenSpaceDistortion = isActive;
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            //只在主相机画，我们也要确保它最多只画一次
            if (m_ScriptablePass != null && renderingData.cameraData.renderType != CameraRenderType.Overlay)
            {
                renderer.EnqueuePass(m_ScriptablePass);
            }
        }
    }
}