Shader "Custom/SpriteGlowOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineSize ("Outline Size", Float) = 0.01
        _Glow ("Glow Strength", Float) = 3
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Overlay"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend One One
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "GlowPass"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineSize;
            float _Glow;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float SampleAlpha(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 offset = _OutlineSize * _MainTex_TexelSize.xy;

                float a = SampleAlpha(i.uv);

                float expanded =
                    max(max(SampleAlpha(i.uv + float2(offset.x, 0)),
                            SampleAlpha(i.uv - float2(offset.x, 0))),
                        max(SampleAlpha(i.uv + float2(0, offset.y)),
                            SampleAlpha(i.uv - float2(0, offset.y))));

                float outline = saturate(expanded - a);

                float4 baseCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;

                float4 glow = _OutlineColor * outline * _Glow;

                return baseCol + glow;
            }

            ENDHLSL
        }
    }
}
