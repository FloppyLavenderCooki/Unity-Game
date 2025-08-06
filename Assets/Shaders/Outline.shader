Shader "CustomEffects/Outline"
{
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        float _Outline;

        float4 Outline(Varyings input) : SV_Target
        {            
            float4 colour = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);

            if (colour.a > 0.0)
            {
                return float4(colour);
            }
            return float4(0, 0, 0, 0);
        }

    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" } 
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