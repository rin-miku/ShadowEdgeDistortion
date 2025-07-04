using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowEdgeDistortionRendererFeature : ScriptableRendererFeature
{
    public ShadowEdgeDistortionRenderPass shadowEdgeDistortionRenderPass;

    public Material shadowEdgeDistortionMaterial;

    public override void Create()
    {
        shadowEdgeDistortionRenderPass = new ShadowEdgeDistortionRenderPass(shadowEdgeDistortionMaterial);
        shadowEdgeDistortionRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingShadows;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(shadowEdgeDistortionRenderPass);
    }
}
