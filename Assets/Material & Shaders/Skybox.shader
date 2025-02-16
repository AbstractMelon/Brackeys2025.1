Shader "Skybox/Procedural Night Sky Stars" {
    Properties {
        _StarDensity ("Star Density", Range(0, 1)) = 0.5
        _StarTwinkleSpeed ("Star Twinkle Speed", Range(0, 5)) = 2.0
        _MilkyWayIntensity ("Milky Way Intensity", Range(0, 2)) = 0.5
        _NoiseTex ("Noise Texture", 2D) = "white" {}
    }

    SubShader {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            float _StarDensity, _StarTwinkleSpeed, _MilkyWayIntensity;
            sampler2D _NoiseTex;

            fixed4 frag(v2f i) : SV_Target {
                float3 viewDir = normalize(i.texcoord);
                
                // Stars calculation
                float2 starUV = viewDir.xy * 50.0 + _Time.x * _StarTwinkleSpeed;
                float starNoise = tex2D(_NoiseTex, starUV).r;
                float stars = pow(starNoise, 10.0 - (8.0 * _StarDensity));
                
                // Milky Way effect
                float2 milkyUV = viewDir.xz * 2.0 + float2(_Time.x * 0.1, 0);
                float milky = tex2D(_NoiseTex, milkyUV).r * _MilkyWayIntensity;
                
                // Combine all elements
                float3 finalColor = float3(0, 0, 0);
                finalColor = lerp(finalColor, float3(1,1,1), stars);
                finalColor += milky * 0.3;
                
                return float4(finalColor, 1);
            }
            ENDCG
        }
    }
}
 