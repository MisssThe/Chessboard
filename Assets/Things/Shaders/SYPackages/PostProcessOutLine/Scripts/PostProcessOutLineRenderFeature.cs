using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Things.Shaders.SYPackages.PostProcessOutLine.Scripts
{
    public class PostProcessOutLineRenderFeature : ScriptableRendererFeature
    {
        [Serializable]
        public class Setting
        {
            public LayerMask characterLayer;

            [Range(1000, 5000)]
            public int queueMin = 2000;
            [Range(1000, 5000)]
            public int queueMax = 3000;

            public Material depthNormalMaterial;
            public Material outLineMaterial;
        }
        public Setting setting = new Setting();

        private DepthNormalsPass m_DepthNormalPass;
        private PostProcessOutLineRenderPass m_DrawOutLinePass;

        public override void Create()
        {
            m_DepthNormalPass = new DepthNormalsPass(setting);
            m_DepthNormalPass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;

            m_DrawOutLinePass = new PostProcessOutLineRenderPass(setting);
            m_DrawOutLinePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }
    
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_DrawOutLinePass.SetUp(renderer.cameraColorTarget);
            
            renderer.EnqueuePass(m_DepthNormalPass);
            renderer.EnqueuePass(m_DrawOutLinePass);
        }
    }

    public class DepthNormalsPass : ScriptableRenderPass
    {
        private readonly string m_PassName = "DrawDepthNormal";
        private readonly ShaderTagId m_ShaderTag = new ShaderTagId("UniversalForward");

        private Material m_Material;
        private FilteringSettings m_FilteringSetting;
        
        public DepthNormalsPass(PostProcessOutLineRenderFeature.Setting setting)
        {
            RenderQueueRange queue = new RenderQueueRange();
            queue.lowerBound = Mathf.Min(setting.queueMax, setting.queueMin);
            queue.upperBound = Mathf.Max(setting.queueMax, setting.queueMin);
            
            m_FilteringSetting = new FilteringSettings(queue, setting.characterLayer);
            m_Material = setting.depthNormalMaterial;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.name = m_PassName;
            
            RenderTextureDescriptor desc = cameraTextureDescriptor;
            desc.width  = desc.width  >> 0;
            desc.height = desc.height >> 0;
            desc.colorFormat = RenderTextureFormat.ARGB32;
            
            int temp = Shader.PropertyToID("_DepthNormalTexture");
            
            cmd.ReleaseTemporaryRT(temp);
            cmd.GetTemporaryRT(temp, desc);
            ConfigureTarget(temp);
            ConfigureClear(ClearFlag.All, Color.black);
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var drawingSettings = CreateDrawingSettings(m_ShaderTag, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            drawingSettings.overrideMaterial = m_Material;
            drawingSettings.overrideMaterialPassIndex = 0;
            
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSetting);
        }
    }

    public class PostProcessOutLineRenderPass : ScriptableRenderPass
    {
        [Header("Tag")]
        private static readonly string outlineRenderTag = "PostProcess OutLine";
        private readonly int m_DepthNormalRT = Shader.PropertyToID("_DepthNormalTexture");

        private RenderTargetIdentifier m_CurrentTarget;
        private Material m_OutLineMaterial;
    
        public PostProcessOutLineRenderPass(PostProcessOutLineRenderFeature.Setting setting)
        {
            this.m_OutLineMaterial = setting.outLineMaterial;
        }

        public void SetUp(RenderTargetIdentifier colorTarget)
        {
            this.m_CurrentTarget = colorTarget;
        }
    
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Draw OutLine
            var cmd = CommandBufferPool.Get(outlineRenderTag);
    
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
        
            CommandBufferPool.Release(cmd);
        }
    
        private void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (m_OutLineMaterial != null)
            {
                var desc = renderingData.cameraData.cameraTargetDescriptor;
                var dest = RenderTexture.GetTemporary(desc);

                cmd.Blit(m_CurrentTarget, dest, m_OutLineMaterial);
                cmd.Blit(dest, m_CurrentTarget);
                
                RenderTexture.ReleaseTemporary(dest);
            }
        }
    }
}