Shader "Skybox/URP Night Sky"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.05, 0.05, 0.2, 1)
        _BottomColor ("Bottom Color", Color) = (0.01, 0.01, 0.05, 1)
        _MoonColor ("Moon Color", Color) = (0.9, 0.9, 0.8, 1)
        _MoonDirection ("Moon Direction", Vector) = (0, 0.5, 0.5)
        _MoonSize ("Moon Size", Range(0,0.1)) = 0.005
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _WAVING_STARS
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct VertexInput
            {
                float4 vertex : POSITION;
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float3 viewDir : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _TopColor;
                float4 _BottomColor;
                float4 _MoonColor;
                float3 _MoonDirection;
                float _MoonSize;
            CBUFFER_END

            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                o.position = TransformObjectToHClip(v.vertex.xyz);
                float3 worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.viewDir = worldPos;
                return o;
            }

            half4 frag(VertexOutput i) : SV_Target
            {
                float3 viewDir = normalize(i.viewDir);

                // Sky gradient
                float gradient = saturate(viewDir.y * 0.5 + 0.5);
                float3 color = lerp(_BottomColor.rgb, _TopColor.rgb, gradient);

                // Moon
                float3 moonDir = normalize(_MoonDirection);
                float moonDot = dot(viewDir, moonDir);
                float moon = smoothstep(1.0 - _MoonSize - 0.001, 1.0 - _MoonSize, moonDot);
                color = lerp(color, _MoonColor.rgb, moon);

                return half4(color, 1.0);
            }
            ENDHLSL
        }
    }
}

