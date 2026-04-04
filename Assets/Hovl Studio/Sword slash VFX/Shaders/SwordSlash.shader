Shader "Hovl/Particles/SwordSlash"
{
    Properties
    {
        _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
        _MainTexture("MainTexture", 2D) = "white" {}
        _EmissionTex("EmissionTex", 2D) = "white" {}
        _Opacity("Opacity", Float) = 20
        _Dissolve("Dissolve", 2D) = "white" {}
        _SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
        _Emission("Emission", Float) = 5
        _Remap("Remap", Vector) = (-2,1,0,0)
        _AddColor("Add Color", Color) = (0,0,0,0)
        _Desaturation("Desaturation", Float) = 0
        [Toggle] _Usedepth ("Use depth?", Float ) = 0
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

            TEXTURE2D(_MainTexture); SAMPLER(sampler_MainTexture);
            TEXTURE2D(_EmissionTex); SAMPLER(sampler_EmissionTex);
            TEXTURE2D(_Dissolve); SAMPLER(sampler_Dissolve);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTexture_ST;
                float4 _EmissionTex_ST;
                float4 _Dissolve_ST;
                float4 _SpeedMainTexUVNoiseZW;
                half4 _AddColor;
                float4 _Remap;
                half _Opacity;
                half _Emission;
                half _Desaturation;
                half _InvFade;
                half _Usedepth;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                half4 color : COLOR;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = pos.positionCS;
                OUT.color = IN.color;
                OUT.uv0 = IN.uv0;
                OUT.uv1 = IN.uv1;
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
                return saturate((sceneEye - partEye) / max(_InvFade, 1e-5));
            }

            float3 RGBToHSV_Custom(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 HSVToRGB_Custom(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = IN.color;
                half fade = lerp(1.0h, SoftFade(IN.screenPos), _Usedepth);
                color.a *= fade;

                float2 emissionUV = IN.uv0.xy * _EmissionTex_ST.xy + _EmissionTex_ST.zw;
                float3 emissionHSV = RGBToHSV_Custom(SAMPLE_TEXTURE2D(_EmissionTex, sampler_EmissionTex, emissionUV).rgb);

                float4 uv1Main = IN.uv1;
                uv1Main.xy = IN.uv1.xy * _MainTexture_ST.xy + _MainTexture_ST.zw;
                float3 shiftedRGB = HSVToRGB_Custom(float3(emissionHSV.x + uv1Main.z, emissionHSV.y, emissionHSV.z));
                float gray = dot(shiftedRGB, float3(0.299, 0.587, 0.114));
                float3 desat = lerp(shiftedRGB, gray.xxx, _Desaturation);
                float3 remapped = clamp(_Remap.x + (desat - 0.0) * (_Remap.y - _Remap.x) / max(1.0, 1e-5), 0.0, 1.0);

                float2 mainPanner = _Time.y * _SpeedMainTexUVNoiseZW.xy + uv1Main.xy;
                float alphaMain = clamp(SAMPLE_TEXTURE2D(_MainTexture, sampler_MainTexture, mainPanner).a * _Opacity, 0.0, 1.0);

                float2 dissolveBase = IN.uv0.xy * _Dissolve_ST.xy + _Dissolve_ST.zw;
                float2 dissolvePanner = _Time.y * _SpeedMainTexUVNoiseZW.zw + dissolveBase;
                float2 dissolveUV = float2(dissolvePanner.x, uv1Main.w + dissolvePanner.y);
                float stepVal = step(1.0 - IN.uv0.x, IN.uv0.w);
                float W = IN.uv0.z;
                float dissolveGate = SAMPLE_TEXTURE2D(_Dissolve, sampler_Dissolve, dissolveUV).r >= (stepVal * W) ? 0.0 : 1.0;

                float3 rgb = ((_AddColor * color) + (_Emission * float4(remapped, 0.0) * color)).rgb;
                float alpha = color.a * alphaMain * dissolveGate;
                return half4(rgb, alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
