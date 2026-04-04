Shader "Hovl/Particles/Blend_CenterGlow"
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
        [Toggle]_Usecenterglow("Use center glow?", Float) = 0
        [Toggle] _Usedepth ("Use depth?", Float ) = 0
        _Depthpower ("Depth power", Float ) = 1
        [Enum(Cull Off,0, Cull Front,1, Cull Back,2)] _CullMode("Culling", Float) = 0
    }

    SubShader
    {
        Tags{ "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        Cull [_CullMode]
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
                half _Usecenterglow;
                half _Usedepth;
                half _Depthpower;
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

            half SoftParticleFade(float4 screenPos)
            {
                float2 uv = screenPos.xy / screenPos.w;
                #if UNITY_REVERSED_Z
                real rawDepth = SampleSceneDepth(uv);
                #else
                real rawDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1.0, SampleSceneDepth(uv));
                #endif
                float sceneEye = LinearEyeDepth(rawDepth, _ZBufferParams);
                float partEye = LinearEyeDepth(screenPos.z / screenPos.w, _ZBufferParams);
                float fade = saturate((sceneEye - partEye) / max(_Depthpower, 1e-4));
                return lerp(1.0h, (half)fade, _Usedepth);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 particleColor = IN.color;
                particleColor.a *= SoftParticleFade(IN.screenPos);

                float2 mainPan = IN.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw + _Time.y * _SpeedMainTexUVNoiseZW.xy;
                float2 flowPan = IN.uv.xy * _Flow_ST.xy + _Flow_ST.zw + _Time.y * _DistortionSpeedXYPowerZ.xy;
                float2 noisePan = IN.uv.xy * _Noise_ST.xy + _Noise_ST.zw + _Time.y * _SpeedMainTexUVNoiseZW.zw;
                half4 maskTex = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, IN.uv.xy * _Mask_ST.xy + _Mask_ST.zw);
                half2 flow = (SAMPLE_TEXTURE2D(_Flow, sampler_Flow, flowPan) * maskTex).rg * _DistortionSpeedXYPowerZ.z;
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, mainPan - flow);
                half4 noiseTex = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, noisePan);

                half4 baseFx = mainTex * noiseTex * _Color * particleColor * mainTex.a * noiseTex.a * _Color.a * particleColor.a;
                half centerRemap = saturate(maskTex.r - (1.0h - IN.uv.z));
                half centerGlow = saturate(maskTex.r * centerRemap);
                half4 result = lerp(baseFx, baseFx * centerGlow, _Usecenterglow) * _Emission;
                return half4(result.rgb, result.a);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
