Shader "UI/FearVignettePixel_URP"
{
    Properties
    {
        _Color ("Dark Color", Color) = (0,0,0,1)
        _Center ("Center (Viewport 0..1)", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Radius", Range(0,1)) = 0.6
        _Softness ("Softness", Range(0.001, 0.5)) = 0.08
        _PixelSize ("Pixel Size", Range(50, 2000)) = 600
        _Darkness ("Darkness", Range(0,1)) = 0.9
        _NoiseScale ("Noise Scale", Range(1, 50)) = 18
        _NoiseStrength ("Noise Strength", Range(0, 0.4)) = 0.12
        _SmokeSpeed ("Smoke Speed", Range(0, 2)) = 0.25
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }

        Pass
        {
            Name "UI_FearVignette"
            Tags { "LightMode"="SRPDefaultUnlit" }

            Cull Off
            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _Center;
                float _Radius;
                float _Softness;
                float _PixelSize;
                float _Darkness;
                float _NoiseScale;
                float _NoiseStrength;
                float _SmokeSpeed;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                return OUT;
            }

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float2 puv = floor(uv * _PixelSize) / _PixelSize;

                float2 nUV = puv * _NoiseScale;
                nUV += float2(_Time.y * _SmokeSpeed, _Time.y * _SmokeSpeed * 0.7);

                float n = hash21(floor(nUV));
                n = (n - 0.5) * 2.0;

                float2 c = _Center.xy;

                float2 p = puv - c;
                float aspect = _ScreenParams.x / _ScreenParams.y;
                p.x *= aspect;

                float d = length(p);

                float noisyRadius = _Radius + n * _NoiseStrength;

                float mask = smoothstep(noisyRadius, noisyRadius + _Softness, d);
                float alpha = mask * _Darkness;

                return half4(_Color.rgb, _Color.a * alpha);
            }
            ENDHLSL
        }
    }
}