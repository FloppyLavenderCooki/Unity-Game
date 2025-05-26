Shader "CustomEffects/Pass"
{
    HLSLINCLUDE
    
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        // The Blit.hlsl file provides the vertex shader (Vert),
        // the input structure (Attributes), and the output structure (Varyings)
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        
        float4 FragmentPass(Varyings input) : SV_Target
        {
            return float4(SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).rgba);
        }
    
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "PassPass"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment FragmentPass
            
            ENDHLSL
        }
    }
}
