Shader "CustomEffects/Blur"
{
    HLSLINCLUDE
    
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        // The Blit.hlsl file provides the vertex shader (Vert),
        // the input structure (Attributes), and the output structure (Varyings)
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        float _Blur;
    
        float4 Blur(Varyings input) : SV_Target
        {
            _Blur *= length(input.texcoord - 0.5) / length(2.5);
            float4 colour = float4(0,0,0,0);

            for (int y = -1; y < 2; ++y)
            {
                for (int x = -1; x < 2; ++x)
                {
                    colour += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord + (float2(x, y) * _Blur));
                }
            }
            colour /= 9;

            return colour;
        }
    
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "BlurPass"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment Blur
            
            ENDHLSL
        }
    }
}
