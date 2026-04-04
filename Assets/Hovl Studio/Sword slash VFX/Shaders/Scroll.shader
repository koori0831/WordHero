Shader "Hovl/Particles/Scroll"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _Noise("Noise", 2D) = "white" {}
        _Flow("Flow", 2D) = "white" {}
        _Mask("Mask", 2D) = "white" {}
        _SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
        _DistortionSpeedXYPowerZ("Distortion Speed XY Power Z", Vector) = (0,0,0,0)
        _Emission("Emission", Float) = 2
        _Color("Color", Color) = (0.5,0.5,0.5,1)
        _Opacity("Opacity", Range(0,1)) = 1
        _PathSet0ifyouuseinPS("Path(Set 0 if you use in PS)", Range(0,1)) = 0
        _Noisedistortpower("Noise distort power", Float) = 1
        [Toggle]_UsePScustomdataW("Use PS custom data W", Float) = 1
        [Toggle] _Usedepth ("Use depth?", Float ) = 0
        _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
    }

    SubShader
    {
        Tags{ "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        Cull Off
        ZWrite Off
        ZTest LEqual

        Pass
        {
            Name "Forward"
            Tags{ "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_Noise); SAMPLER(sampler_Noise);
            TEXTURE2D(_Flow); SAMPLER(sampler_Flow);
            TEXTURE2D(_Mask); SAMPLER(sampler_Mask);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Noise_ST;
                float4 _Flow_ST;
                float4 _Mask_ST;
                half4 _Color;
                float4 _SpeedMainTexUVNoiseZW;
                float4 _DistortionSpeedXYPowerZ;
                half _Emission;
                half _Opacity;
                half _PathSet0ifyouuseinPS;
                half _Noisedistortpower;
                half _UsePScustomdataW;
                half _Usedepth;
                half _InvFade;
            CBUFFER_END

            struct Attributes { float4 positionOS:POSITION; half4 color:COLOR; float4 uv:TEXCOORD0; };
            struct Varyings { float4 positionCS:SV_POSITION; half4 color:COLOR; float4 uv:TEXCOORD0; float4 screenPos:TEXCOORD1; };

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
                real rawDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1.0, SampleSceneDepth(uv));
                #endif
                float sceneEye = LinearEyeDepth(rawDepth, _ZBufferParams);
                float partEye = LinearEyeDepth(screenPos.z / screenPos.w, _ZBufferParams);
                float fade = saturate(_InvFade * (sceneEye - partEye));
                return lerp(1.0h, (half)fade, _Usedepth);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 particleColor = IN.color;
                particleColor.a *= SoftFade(IN.screenPos);

                float2 mainUV = IN.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw + _Time.y * _SpeedMainTexUVNoiseZW.xy;
                float2 flowUV = IN.uv.xy * _Flow_ST.xy + _Flow_ST.zw + _Time.y * _DistortionSpeedXYPowerZ.xy;
                half2 flow = SAMPLE_TEXTURE2D(_Flow, sampler_Flow, flowUV).rg * _DistortionSpeedXYPowerZ.z;

                float2 noiseUV = IN.uv.xy * _Noise_ST.xy + _Noise_ST.zw + _Time.y * _SpeedMainTexUVNoiseZW.zw;
                half W = IN.uv.z;
                float pathBase = saturate(_PathSet0ifyouuseinPS + W);
                float denom = max(1.0 - pathBase, 1e-4);
                float2 remappedNoiseUV = float2(noiseUV.x, (noiseUV.y - pathBase) / denom);
                half4 noiseTex = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, remappedNoiseUV - flow);
                half4 noiseClamp = saturate(noiseTex - 0.1h);
                half topFade = saturate(1.0h - pow(1.0h - IN.uv.y, 40.0h));
                half alphaBase = saturate(noiseTex.a - 0.1h);
                half customW = saturate(IN.uv.w);
                half exponent = _Noisedistortpower + lerp(0.0h, customW * 10.0h, _UsePScustomdataW);
                half powAlpha = saturate(pow(alphaBase, exponent));
                half alphaRamp = saturate(-2.0h + powAlpha * 7.0h);
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, mainUV - flow);
                half maskA = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, IN.uv.xy * _Mask_ST.xy + _Mask_ST.zw).a;

                half3 rgb = (mainTex * noiseClamp * _Color * particleColor * topFade).rgb * _Emission;
                half alpha = alphaRamp * topFade * _Color.a * particleColor.a * _Opacity * maskA;
                return half4(rgb, alpha);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
