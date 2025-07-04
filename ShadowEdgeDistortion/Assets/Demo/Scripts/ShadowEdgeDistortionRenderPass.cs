using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class ShadowEdgeDistortionRenderPass : ScriptableRenderPass
{
    private Material shadowEdgeDistortionMaterial;

    public ShadowEdgeDistortionRenderPass(Material material)
    {
        shadowEdgeDistortionMaterial = material;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

        Debug.Log(resourceData.mainShadowsTexture.GetDescriptor(renderGraph).name);

        base.RecordRenderGraph(renderGraph, frameData);
    }
}
