Shader "Hovl/Particles/Explosion"
{
    Properties
    {
        _Noise("Noise", 2D) = "white" {}
        _FinalEmission("Final Emission", Float) = 1
        _Color("Color", Color) = (1,1,1,1)
        _GlowColor("Glow Color", Color) = (1,1,0,1)
        _Opacity("Opacity", Range(0, 1)) = 1
        _NoisespeedXYNoisepowerZGlowpowerW("Noise speed XY Noise power Z Glow power W", Vector) = (0.314,0.427,0.001,4)
        _MotionVector("MotionVector", 2D) = "white" {}
        _MainTex("MainTex", 2D) = "white" {}
        _TilingXY("Tiling XY", Vector) = (8,8,0,0)
        _MotionAmount("MotionAmount", Float) = 0.001
        [Toggle] _Usedepth ("Use depth?", Float ) = 0
        _Depthpower ("Depth power", Float ) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        Cull Off
        ZWrite Off
        ZTest LEqual

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_Noise); SAMPLER(sampler_Noise);
            TEXTURE2D(_MotionVector); SAMPLER(sampler_MotionVector);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Noise_ST;
                half4 _GlowColor;
                half4 _Color;
                float4 _NoisespeedXYNoisepowerZGlowpowerW;
                float4 _TilingXY;
                half _MotionAmount;
                half _FinalEmission;
                half _Opacity;
                half _Usedepth;
                half _Depthpower;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                half4 color : COLOR;
                float4 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
                float4 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = pos.positionCS;
                OUT.color = IN.color;
                OUT.uv = IN.uv;
                OUT.screenPos = ComputeScreenPos(pos.positionCS);
                return OUT;
            }

            half SoftFade(float4 screenPos)
            {
                float2 uv = screenPos.xy / screenPos.w;
                #if UNITY_REVERSED_Z
                    real rawDepth = SampleSceneDepth(uv);
                #else
                    real rawDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(uv));
                #endif
                float sceneEye = LinearEyeDepth(rawDepth, _ZBufferParams);
                float partEye = LinearEyeDepth(screenPos.z / screenPos.w, _ZBufferParams);
                return saturate((sceneEye - partEye) / max(_Depthpower, 1e-5));
            }

            float2 FlipbookUV(float2 baseUV, float frame, float2 tilingXY)
            {
                float total = max(tilingXY.x * tilingXY.y, 1.0);
                float cols = max(tilingXY.x, 1.0);
                float rows = max(tilingXY.y, 1.0);
                float2 tileScale = float2(1.0 / cols, 1.0 / rows);
                float index = round(fmod(frame, total));
                index += (index < 0.0) ? total : 0.0;
                float x = round(fmod(index, cols));
                float y = round(fmod((index - x) / cols, rows));
                y = (rows - 1.0) - y;
                return baseUV * tileScale + float2(x * tileScale.x, y * tileScale.y);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = IN.color;
                half fade = lerp(1.0h, SoftFade(IN.screenPos), _Usedepth);
                color.a *= fade;

                float2 noiseUV = IN.uv.xy * _Noise_ST.xy + _Noise_ST.zw;
                float emissionMask = IN.uv.z;
                float t = IN.uv.w;
                float fracT = frac(t);
                float frame0 = floor(t);
                float frame1 = frame0 + 1.0;

                float2 noiseScroll = _Time.y * _NoisespeedXYNoisepowerZGlowpowerW.xy;
                half4 noiseSample = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, noiseUV + noiseScroll) * _NoisespeedXYNoisepowerZGlowpowerW.z;

                float2 mainUV = IN.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 fbuv0 = FlipbookUV(mainUV, frame0, _TilingXY.xy);
                float2 fbuv1 = FlipbookUV(mainUV, frame1, _TilingXY.xy);

                half4 motion = SAMPLE_TEXTURE2D(_MotionVector, sampler_MotionVector, fbuv0);
                motion = _MotionAmount + (motion - 0.0h) * ((-_MotionAmount) - _MotionAmount) / (1.0h - 0.0h);

                half4 tex0 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, (noiseSample + fracT * motion + half4(fbuv0, 0, 0)).rg);
                half4 tex1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, (noiseSample + (fracT - 1.0) * motion + half4(fbuv1, 0, 0)).rg);
                half4 blendTex = lerp(tex0, tex1, fracT);

                half glowPower = max(_NoisespeedXYNoisepowerZGlowpowerW.w, 0.0);
                half4 glow = clamp(_GlowColor * emissionMask * pow(abs(blendTex), glowPower), 0.0h, 10000.0h);
                half alpha = lerp(tex0.a, tex1.a, fracT) * _Color.a * color.a * _Opacity;
                half3 rgb = ((glow + blendTex) * _Color * color * _FinalEmission).rgb;
                return half4(rgb, alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
