Shader "Custom/ShadowDistortion"
{
    Properties
    {
        _DistortionAmount("_DistortionAmount", Range(0, 5)) = 1.0
        _PixelSize("_PixelSize", Float) = 200.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off 
        ZWrite Off
        Pass
        {
            Name "ShadowDistortion"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ImageBasedLighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            float _DistortionAmount;
            float _PixelSize;

            float Noise(float2 uv)
            {
                return frac(sin(dot(uv * 1234.56 + _Time.y * 10, float2(12.9898, 78.233))) * 43758.5453) * 2 - 1;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float2 screenUV = GetNormalizedScreenSpaceUV(input.positionCS);

                float time = _Time.y;

                float2 distortedUV = screenUV;

                distortedUV.x += sin(time * 10.0 + screenUV.y * 100.0) * 0.002 * _DistortionAmount;  
                distortedUV.y += Noise(screenUV * 10.0 + time * 3.0) * 0.002 * _DistortionAmount;

                float2 pixelSize = float2(_PixelSize, _PixelSize);
                distortedUV = floor(distortedUV * pixelSize) / pixelSize;
                float pushAmount = (screenUV.x - 0.5) * 0.05 * _DistortionAmount;
                distortedUV.x -= pushAmount;

                half shadowMapDistorted = SAMPLE_TEXTURE2D(_ScreenSpaceShadowmapTexture, sampler_PointClamp, distortedUV);

                return shadowMapDistorted;
            }
            ENDHLSL
        }
    }
}