Shader "CustomEffects/Aberration"
{
    HLSLINCLUDE
    
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        // The Blit.hlsl file provides the vertex shader (Vert),
        // the input structure (Attributes), and the output structure (Varyings)
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        float _Aberration;
    
        float4 Aberration(Varyings input) : SV_Target
        {
            _Aberration *= length(input.texcoord - 0.5) / length(2.5);
            
            float2 dir = -sign(input.texcoord - 0.5);

            float2 rOffset = dir * _Aberration;
            float2 bOffset = -dir * _Aberration;
            
            float3 colour = float3(
                SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord + rOffset).r,
                SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).g,
                SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord + bOffset).b);

            return float4(colour, SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).a);
        }
    
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "AberrationPass"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment Aberration
            
            ENDHLSL
        }
    }
}
