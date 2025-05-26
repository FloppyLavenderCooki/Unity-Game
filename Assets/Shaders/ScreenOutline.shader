Shader "CustomEffects/ScreenOutline"
{
    HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;

        float4 Outline(Varyings input) : SV_Target
        {
            float2 uv = input.texcoord;
            float center = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).r;
            float up     = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, _MainTex_TexelSize.y)).r;
            float down   = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - float2(0, _MainTex_TexelSize.y)).r;
            float left   = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - float2(_MainTex_TexelSize.x, 0)).r;
            float right  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(_MainTex_TexelSize.x, 0)).r;

            float outline = step(0.1, up + down + left + right) * step(center, 0.01);

            return float4(0, 1, 0, outline);
        }
    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZWrite Off Cull Off

        Pass
        {
            Name "OutlinePass"
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Outline
            ENDHLSL
        }
    }
}
