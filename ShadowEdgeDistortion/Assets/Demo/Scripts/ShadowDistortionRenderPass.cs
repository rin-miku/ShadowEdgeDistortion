using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class ShadowDistortionRenderPass : ScriptableRenderPass
{
    private int shadowTexID;
    private Material shadowDistortionMaterial;

    class PassData
    {
        public Material material;
        public TextureHandle target;
    }

    public ShadowDistortionRenderPass(Material material)
    {
        shadowTexID = Shader.PropertyToID("_ScreenSpaceShadowmapTexture");
        shadowDistortionMaterial = material;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

        var desc = cameraData.cameraTargetDescriptor;
        desc.depthStencilFormat = GraphicsFormat.None;
        desc.msaaSamples = 1;
        desc.graphicsFormat = SystemInfo.IsFormatSupported(GraphicsFormat.R8_UNorm, GraphicsFormatUsage.Blend)
            ? GraphicsFormat.R8_UNorm
            : GraphicsFormat.B8G8R8A8_UNorm;

        TextureHandle color = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_TempScreenSpaceShadowmapTexture", true);

        using (var builder = renderGraph.AddRasterRenderPass<PassData>("ShadowDistortion", out var passData))
        {
            passData.material = shadowDistortionMaterial;
            passData.target = color;

            builder.SetRenderAttachment(color, 0, AccessFlags.Write);
            //builder.UseGlobalTexture(shadowTexID);
            //builder.AllowGlobalStateModification(true);

            builder.SetGlobalTextureAfterPass(color, shadowTexID);

            builder.SetRenderFunc((PassData data, RasterGraphContext context) => 
            {
                Blitter.BlitTexture(context.cmd, data.target, Vector4.one, data.material, 0);
            });
        }
    }
}
