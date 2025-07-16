using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.RenderGraphModule.Util.RenderGraphUtils;

public class ShadowDistortionBlendRenderPass : ScriptableRenderPass
{
    private Material shadowEdgeDistortionMaterial;

    class PassData
    {
        public RendererListHandle rendererListHandle;
    }


    public ShadowDistortionBlendRenderPass(Material material)
    {
        shadowEdgeDistortionMaterial = material;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

        FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque)
        {
            layerMask = LayerMask.GetMask("Ground")
        };
        RendererListDesc rendererListDesc = new RendererListDesc(new ShaderTagId("ShadowDistortion"), renderingData.cullResults, cameraData.camera)
        {
            sortingCriteria = SortingCriteria.CommonOpaque,
            renderQueueRange = RenderQueueRange.opaque,
            layerMask = filteringSettings.layerMask
        };

        RenderTextureDescriptor descriptor = cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;
        descriptor.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
        TextureHandle shadowDistortionRT = UniversalRenderer.CreateRenderGraphTexture(renderGraph, descriptor, "ShadowDistortionRT", false);

        RendererListHandle rendererListHandle = renderGraph.CreateRendererList(rendererListDesc);

        using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass<PassData>("ShadowDistortion", out var passData))
        {
            builder.SetRenderAttachment(shadowDistortionRT, 0, AccessFlags.Write);
            passData.rendererListHandle = rendererListHandle;
            builder.UseRendererList(rendererListHandle);
            builder.AllowPassCulling(false);

            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                context.cmd.DrawRendererList(data.rendererListHandle);
            });
        }

        TextureHandle cameraColor = resourceData.cameraColor;
        BlitMaterialParameters blitParams = new BlitMaterialParameters(shadowDistortionRT, cameraColor, shadowEdgeDistortionMaterial, 0);
        renderGraph.AddBlitPass(blitParams, "Blend ShadowDistortion To CameraColor");
        resourceData.cameraColor = cameraColor;
    }
}
