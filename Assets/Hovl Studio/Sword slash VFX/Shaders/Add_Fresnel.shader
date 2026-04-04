Shader "Hovl/Particles/Add_Fresnel"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _Noise("Noise", 2D) = "white" {}
        _Color("Color", Color) = (0.5,0.5,0.5,1)
        _Emission("Emission", Float) = 2
        _SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
        _Flow("Flow", 2D) = "white" {}
        _Mask("Mask", 2D) = "white" {}
        _Distortionpower("Distortion power", Float) = 0.2
        _Fresnelscale("Fresnel scale", Float) = 3
        _Fresnelpower("Fresnel power", Float) = 3
        _Depthpower("Depth power", Float) = 0.2
        [Toggle]_Useonlycolor("Use only color", Float) = 0
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
                half _Emission;
                half _Distortionpower;
                half _Fresnelscale;
                half _Fresnelpower;
                half _Depthpower;
                half _Useonlycolor;
            CBUFFER_END

            struct Attributes { float4 positionOS:POSITION; float3 normalOS:NORMAL; half4 color:COLOR; float4 uv:TEXCOORD0; };
            struct Varyings {
                float4 positionCS:SV_POSITION; half4 color:COLOR; float4 uv:TEXCOORD0; float3 positionWS:TEXCOORD1; half3 normalWS:TEXCOORD2; float4 screenPos:TEXCOORD3; };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs nrm = GetVertexNormalInputs(IN.normalOS);
                OUT.positionCS = pos.positionCS;
                OUT.positionWS = pos.positionWS;
                OUT.normalWS = normalize(nrm.normalWS);
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
                return saturate(_Depthpower * (sceneEye - partEye));
            }

            half4 frag(Varyings IN, half facing : VFACE) : SV_Target
            {
                half fade = SoftFade(IN.screenPos);
                float2 mainUV = IN.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw + _Time.y * _SpeedMainTexUVNoiseZW.xy;
                float2 flowUV = IN.uv.xy * _Flow_ST.xy + _Flow_ST.zw + _Time.y * _SpeedMainTexUVNoiseZW.zw;
                float2 noiseUV = IN.uv.xy * _Noise_ST.xy + _Noise_ST.zw;
                float2 maskUV = IN.uv.xy * _Mask_ST.xy + _Mask_ST.zw;

                half4 maskTex = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, maskUV);
                half2 flow = (maskTex * SAMPLE_TEXTURE2D(_Flow, sampler_Flow, flowUV)).rg * _Distortionpower;
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, mainUV - flow);
                half4 noiseTex = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, noiseUV);

                half wBands = 1.0h + IN.uv.z * 127.0h;
                half divv = 256.0h / max(floor(wBands), 1.0h);
                half posterizedAlpha = floor(mainTex.a * divv) / divv;

                half3 viewDirWS = SafeNormalize(GetWorldSpaceViewDir(IN.positionWS));
                half ndv = saturate(dot(normalize(IN.normalWS), viewDirWS));
                half fresnel = saturate(_Fresnelscale * pow(1.0h - ndv, _Fresnelpower));
                half frontFresnel = facing > 0 ? fresnel : 0.0h;
                half alphaMul = saturate((1.0h - fade) - frontFresnel);
                alphaMul = saturate(frontFresnel + alphaMul);

                half4 texColor = half4((mainTex * noiseTex * _Color * IN.color).rgb, 0);
                half4 rgbOut = lerp(texColor, _Color, _Useonlycolor) * _Emission;
                half alpha = posterizedAlpha * noiseTex.a * _Color.a * IN.color.a * alphaMul;
                return half4(rgbOut.rgb, alpha);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
