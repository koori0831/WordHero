Shader "Hovl/Particles/DissolveNoise"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _TextureNoise("Texture Noise", 2D) = "white" {}
        _Dissolvenoise("Dissolve noise", 2D) = "white" {}
        _NoisespeedXYEmissonZPowerW("Noise speed XY / Emisson Z / Power W", Vector) = (0.5,0,2,1)
        _DissolvespeedXY("Dissolve speed XY", Vector) = (0,0,0,0)
        _Maincolor("Main color", Color) = (0.7609469,0.8547776,0.9433962,1)
        _Noisecolor("Noise color", Color) = (0.2470588,0.3012382,0.3607843,1)
        _Dissolvecolor("Dissolve color", Color) = (1,1,1,1)
        [Toggle]_Usetexturecolor("Use texture color", Float) = 0
        [Toggle]_Usetexturedissolve("Use texture dissolve", Float) = 0
        _Opacity("Opacity", Range(0,1)) = 1
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
            TEXTURE2D(_TextureNoise); SAMPLER(sampler_TextureNoise);
            TEXTURE2D(_Dissolvenoise); SAMPLER(sampler_Dissolvenoise);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _TextureNoise_ST;
                float4 _Dissolvenoise_ST;
                float4 _NoisespeedXYEmissonZPowerW;
                float4 _DissolvespeedXY;
                half4 _Maincolor;
                half4 _Noisecolor;
                half4 _Dissolvecolor;
                half _Usetexturecolor;
                half _Usetexturedissolve;
                half _Opacity;
                half _Usedepth;
                half _InvFade;
            CBUFFER_END

            struct Attributes { float4 positionOS:POSITION; half4 color:COLOR; float4 uv0:TEXCOORD0; float4 uv1:TEXCOORD1; };
            struct Varyings { float4 positionCS:SV_POSITION; half4 color:COLOR; float4 uv0:TEXCOORD0; float4 uv1:TEXCOORD1; float4 screenPos:TEXCOORD2; };

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

                float3 uv1Dissolve = float3(IN.uv1.xy * _Dissolvenoise_ST.xy + _Dissolvenoise_ST.zw, IN.uv1.z);
                half W = uv1Dissolve.z;
                float2 noiseUV = IN.uv0.xy * _TextureNoise_ST.xy + _TextureNoise_ST.zw + _Time.y * _NoisespeedXYEmissonZPowerW.xy + W + float2(0.2, 0.4);
                half noisePower = _NoisespeedXYEmissonZPowerW.w;
                half4 texNoise = SAMPLE_TEXTURE2D(_TextureNoise, sampler_TextureNoise, noiseUV);
                half4 clampNoise = saturate(pow(texNoise, noisePower) * noisePower);
                half4 edgeColor = lerp(_Maincolor, _Noisecolor, clampNoise);

                float2 dissolveUV = uv1Dissolve.xy + W + _Time.y * _DissolvespeedXY.xy;
                half4 dissolveTex = SAMPLE_TEXTURE2D(_Dissolvenoise, sampler_Dissolvenoise, dissolveUV);
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv0.xy * _MainTex_ST.xy + _MainTex_ST.zw);
                half dissolveSource = lerp(dissolveTex.r, dissolveTex.r * mainTex.r, _Usetexturedissolve);
                half dissolveStep = step(dissolveSource, IN.uv0.z);

                half4 aliveColor = edgeColor * (1.0h - dissolveStep);
                half clamp87 = saturate(((-4.0h + ((((-0.65h + ((1.0h - IN.uv0.z) * 1.3h)) + dissolveSource) * 11.0h)) ) * 3.0h));
                half4 mainBranch = lerp(aliveColor, aliveColor * mainTex, _Usetexturecolor);
                half4 dissolveBranch = lerp(_Dissolvecolor, _Dissolvecolor * mainTex, _Usetexturecolor);
                half4 finalRgb = lerp(mainBranch, dissolveBranch, clamp87 * dissolveStep);
                half clamp99 = saturate(-15.0h + ((dissolveSource + (-0.65h + IN.uv0.w * 1.3h)) * 30.0h));

                half3 rgb = (_NoisespeedXYEmissonZPowerW.z * finalRgb * particleColor).rgb;
                half alpha = particleColor.a * mainTex.a * clamp99 * _Opacity;
                return half4(rgb, alpha);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
