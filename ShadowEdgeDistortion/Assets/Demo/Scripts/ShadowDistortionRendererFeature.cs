using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowDistortionRendererFeature : ScriptableRendererFeature
{
    public ShadowDistortionRenderPass shadowDistortionRenderPass;

    public Material shadowDistortionMaterial;

    public override void Create()
    {
        shadowDistortionRenderPass = new ShadowDistortionRenderPass(shadowDistortionMaterial);
        shadowDistortionRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(shadowDistortionRenderPass);
    }
}
