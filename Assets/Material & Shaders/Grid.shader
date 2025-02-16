Shader "Custom/DynamicGridURP"
{
    Properties
    {
        _GridColor ("Grid Color", Color) = (1,1,1,1)
        _BackgroundColor ("Background Color", Color) = (0,0,0,1)
        _GridSize ("Grid Size", Float) = 1
        _LineWidth ("Line Width", Float) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _GridColor;
            float4 _BackgroundColor;
            float _GridSize;
            float _LineWidth;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Compute grid lines using world position
                float2 grid = abs(frac(IN.worldPos.xz / _GridSize) - 0.5) / _LineWidth;
                float gridIntensity = 1.0 - smoothstep(0.0, 1.0, min(grid.x, grid.y));

                // Return final color with alpha blending
                return lerp(_BackgroundColor, _GridColor, gridIntensity * _GridColor.a);
            }
            ENDHLSL
        }
    }
}
