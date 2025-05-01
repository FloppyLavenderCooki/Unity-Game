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
            float3 colour = float3(
                SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord - _Aberration).r,
                SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).g,
                SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord + _Aberration).b);

            return float4(colour, 1);
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
