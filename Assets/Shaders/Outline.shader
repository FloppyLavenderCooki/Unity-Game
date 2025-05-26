Shader "CustomEffects/Outline"
{
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        float _Outline;
        float4 _OutlineColor;
        float _OutlineThreshold;
        int _MainCamera;

        float4 Outline(Varyings input) : SV_Target
        {            
            float centerAlpha = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).a;

            float2 offsets[4] = {
                float2(_Outline, 0),
                float2(-_Outline, 0),
                float2(0, _Outline * 2),
                float2(0, -_Outline * 2)
            };

            float maxAlphaDiff = 0.0;

            for (int i = 0; i < 4; i++)
            {
                float neighbourAlpha = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord + offsets[i]).a;
                maxAlphaDiff = max(maxAlphaDiff, abs(centerAlpha - neighbourAlpha));
            }

            if (maxAlphaDiff > _OutlineThreshold) 
            {
                return _OutlineColor; 
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
            
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Outline

            ENDHLSL
        }
    }
}