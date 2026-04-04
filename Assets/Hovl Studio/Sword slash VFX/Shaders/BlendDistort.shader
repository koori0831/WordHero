Shader "Hovl/Particles/BlendDistort"
{
    Properties
    {
        [MainTexture]_MainTex("MainTex", 2D) = "white" {}
        _Noise("Noise", 2D) = "white" {}
        _Flow("Flow", 2D) = "white" {}
        _Mask("Mask", 2D) = "white" {}
        [Normal]_NormalMap("NormalMap", 2D) = "bump" {}
        [MainColor]_Color("Color", Color) = (0.5,0.5,0.5,1)
        _Distortionpower("Distortion power", Float) = 0
        _SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
        _DistortionSpeedXYPowerZ("Distortion Speed XY Power Z", Vector) = (0,0,0,0)
        _Emission("Emission", Float) = 2
        _Opacity("Opacity", Range(0, 3)) = 1
        [Toggle]_Usedepth("Use depth?", Float) = 1
        [Toggle]_Softedges("Soft edges", Float) = 0
        _Depthpower("Depth power", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            TEXTURE2D(_MainTex);            SAMPLER(sampler_MainTex);
            TEXTURE2D(_Noise);              SAMPLER(sampler_Noise);
            TEXTURE2D(_Flow);               SAMPLER(sampler_Flow);
            TEXTURE2D(_Mask);               SAMPLER(sampler_Mask);
            TEXTURE2D(_NormalMap);          SAMPLER(sampler_NormalMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Noise_ST;
                float4 _Flow_ST;
                float4 _Mask_ST;
                float4 _NormalMap_ST;
                half4 _Color;
                float4 _SpeedMainTexUVNoiseZW;
                float4 _DistortionSpeedXYPowerZ;
                half _Distortionpower;
                half _Emission;
                half _Opacity;
                half _Usedepth;
                half _Softedges;
                half _Depthpower;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 color      : COLOR;
                float2 uv0        : TEXCOORD0;
                float4 uv4        : TEXCOORD4;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv0        : TEXCOORD0;
                float4 uv4        : TEXCOORD1;
                half4 color       : COLOR;
                float3 positionWS : TEXCOORD2;
                half3 normalWS    : TEXCOORD3;
                float4 screenPos  : TEXCOORD4;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS = posInputs.positionCS;
                OUT.positionWS = posInputs.positionWS;
                OUT.normalWS = normalize(normalInputs.normalWS);
                OUT.uv0 = IN.uv0;
                OUT.uv4 = IN.uv4;
                OUT.color = IN.color;
                OUT.screenPos = ComputeScreenPos(posInputs.positionCS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float timeY = _Time.y;

                float2 mainSpeed = _SpeedMainTexUVNoiseZW.xy;
                float2 noiseSpeed = _SpeedMainTexUVNoiseZW.zw;
                float2 distortionSpeed = _DistortionSpeedXYPowerZ.xy;
                float flowPower = _DistortionSpeedXYPowerZ.z;

                float2 uvNormal = TRANSFORM_TEX(IN.uv0, _NormalMap);
                float2 pannerNormal = uvNormal + noiseSpeed * timeY;

                half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, pannerNormal), _Distortionpower);

                float2 screenUV = IN.screenPos.xy / max(IN.screenPos.w, 1e-6);
                float2 distortedScreenUV = screenUV + normalTS.xy;
                half3 screenColor = SampleSceneColor(distortedScreenUV);

                float2 uvMain = TRANSFORM_TEX(IN.uv0, _MainTex);
                float2 pannerMain = uvMain + mainSpeed * timeY;

                float2 uvFlow = IN.uv4.xy * _Flow_ST.xy + _Flow_ST.zw;
                float2 pannerFlow = uvFlow + distortionSpeed * timeY;
                half4 flowTex = SAMPLE_TEXTURE2D(_Flow, sampler_Flow, pannerFlow);

                float2 uvMask = TRANSFORM_TEX(IN.uv0, _Mask);
                half4 maskTex = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uvMask);

                half2 distortedMainUV = pannerMain - ((flowTex * maskTex) * flowPower).rg;
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedMainUV);

                float2 uvNoise = TRANSFORM_TEX(IN.uv0, _Noise);
                float2 pannerNoise = uvNoise + noiseSpeed * timeY;
                float2 noiseOffset = float2(IN.uv4.w, 0.0);
                half4 noiseTex = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, pannerNoise + noiseOffset);

                half alphaBase = saturate(mainTex.a * noiseTex.a * _Color.a * IN.color.a * _Opacity);
                half3 emissiveParticle = ((mainTex * noiseTex * _Color * IN.color) * _Emission * alphaBase).rgb;

                half blendMode = saturate(IN.uv4.z);
                half3 combined = lerp(screenColor + emissiveParticle, screenColor * emissiveParticle, blendMode);

                float2 depthUV = screenUV;
                #if UNITY_REVERSED_Z
                    real rawDepth = SampleSceneDepth(depthUV);
                #else
                    real rawDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(depthUV));
                #endif
                float sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
                float thisEyeDepth = LinearEyeDepth(IN.positionCS.z / IN.positionCS.w, _ZBufferParams);
                float depthFade = saturate(abs(sceneEyeDepth - thisEyeDepth) / max(_Depthpower, 1e-5));

                half3 n = normalize(IN.normalWS);
                half3 v = normalize(GetWorldSpaceViewDir(IN.positionWS));
                half ndotv = dot(n, v);
                half softEdgeFactor = saturate((pow(ndotv, 3.0h) * 5.0h) * (ndotv >= 0.0h ? 1.0h : 0.0h));

                half alphaDepth = lerp(alphaBase, alphaBase * depthFade, _Usedepth);
                half finalAlpha = lerp(alphaDepth, alphaDepth * softEdgeFactor, _Softedges);

                return half4(combined, finalAlpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
