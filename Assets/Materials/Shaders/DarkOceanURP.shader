Shader "Custom/DarkOceanURP"
{
    Properties
    {
        // Colors
        _NearColor ("Near Color", Color) = (0.1, 0.3, 0.5, 1)   // close to camera
        _FarColor  ("Far Color", Color)  = (0.0, 0.02, 0.08, 1) // far away / deep
        _FoamColor ("Foam Color", Color) = (0.9, 0.95, 1.0, 1)

        // Distance-based fade
        _DepthFadeDistance ("Depth Fade Distance", Float) = 80

        // Wave settings
        _WaveAmplitude1 ("Wave Amplitude 1", Float) = 0.5
        _WaveAmplitude2 ("Wave Amplitude 2", Float) = 0.25
        _WaveAmplitude3 ("Wave Amplitude 3", Float) = 0.15

        _WaveFrequency1 ("Wave Frequency 1", Float) = 0.6
        _WaveFrequency2 ("Wave Frequency 2", Float) = 1.1
        _WaveFrequency3 ("Wave Frequency 3", Float) = 1.8

        _WaveSpeed1 ("Wave Speed 1", Float) = 0.6
        _WaveSpeed2 ("Wave Speed 2", Float) = 1.0
        _WaveSpeed3 ("Wave Speed 3", Float) = 1.7

        _WaveDir1 ("Wave Dir 1 (XZ)", Vector) = (1, 0, 0, 0)
        _WaveDir2 ("Wave Dir 2 (XZ)", Vector) = (0.6, 0.8, 0, 0)
        _WaveDir3 ("Wave Dir 3 (XZ)", Vector) = (-0.8, 0.4, 0, 0)

        // Foam
        _FoamHeight    ("Foam Height Threshold", Float) = 0.3
        _FoamSharpness ("Foam Sharpness", Float) = 3.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
            "RenderPipeline"="UniversalRenderPipeline"
        }
        LOD 200

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float  waveHeight  : TEXCOORD1;   // <- total displacement
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _NearColor;
                float4 _FarColor;
                float4 _FoamColor;

                float  _DepthFadeDistance;

                float  _WaveAmplitude1;
                float  _WaveAmplitude2;
                float  _WaveAmplitude3;

                float  _WaveFrequency1;
                float  _WaveFrequency2;
                float  _WaveFrequency3;

                float  _WaveSpeed1;
                float  _WaveSpeed2;
                float  _WaveSpeed3;

                float4 _WaveDir1;   // XZ
                float4 _WaveDir2;   // XZ
                float4 _WaveDir3;   // XZ

                float  _FoamHeight;
                float  _FoamSharpness;
            CBUFFER_END

            // Simple helper: one sine wave based on world XZ
            float WaveHeight(float2 worldXZ, float2 dir, float amp, float freq, float speed, float time)
            {
                dir = normalize(dir);
                float phase = dot(worldXZ, dir) * freq + time * speed;
                return sin(phase) * amp;
            }

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                // object to world
                float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);
                float2 worldXZ = posWS.xz;

                float t = _Time.y; // Unity time in seconds

                // sum of 3 waves
                float h1 = WaveHeight(worldXZ, _WaveDir1.xz, _WaveAmplitude1, _WaveFrequency1, _WaveSpeed1, t);
                float h2 = WaveHeight(worldXZ, _WaveDir2.xz, _WaveAmplitude2, _WaveFrequency2, _WaveSpeed2, t);
                float h3 = WaveHeight(worldXZ, _WaveDir3.xz, _WaveAmplitude3, _WaveFrequency3, _WaveSpeed3, t);

                float totalHeight = h1 + h2 + h3;

                // displace Y only (assumes plane in XZ)
                posWS.y += totalHeight;

                OUT.positionWS  = posWS;
                OUT.positionHCS = TransformWorldToHClip(posWS);
                OUT.waveHeight  = totalHeight;   // pass to fragment
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // -------- base dark-ocean color (distance fade) --------
                float dist = distance(_WorldSpaceCameraPos.xyz, IN.positionWS);
                float d01  = saturate(dist / max(_DepthFadeDistance, 0.0001));

                float3 baseCol = lerp(_NearColor.rgb, _FarColor.rgb, d01);

                // -------- foam only on high crests, not everywhere -----
                // Use the wave displacement, not absolute world Y
                float crest = max(IN.waveHeight, 0.0); // only upper half of the wave

                // where crest is higher than threshold, add foam
                float foamMask = saturate((crest - _FoamHeight) * _FoamSharpness);

                // tone it down so it never fully replaces the water
                foamMask *= 0.5;  // 0–0.5 influence

                float3 finalCol = lerp(baseCol, _FoamColor.rgb, foamMask);

                return half4(finalCol, 1.0);
            }

            ENDHLSL
        }
    }

    FallBack Off
}
