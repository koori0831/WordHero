Shader "Hovl/Particles/Blend_TwoSides"
{
    Properties
    {
        _Cutoff("Mask Clip Value", Float) = 0.5
        _MainTex("Main Tex", 2D) = "white" {}
        _Mask("Mask", 2D) = "white" {}
        _Noise("Noise", 2D) = "white" {}
        _SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
        _FrontFacesColor("Front Faces Color", Color) = (0,0.2313726,1,1)
        _BackFacesColor("Back Faces Color", Color) = (0.1098039,0.4235294,1,1)
        _Emission("Emission", Float) = 2
        [Toggle]_UseFresnel("Use Fresnel?", Float) = 1
        [Toggle]_SeparateFresnel("SeparateFresnel", Float) = 0
        _SeparateEmission("Separate Emission", Float) = 2
        [HDR]_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        _Fresnel("Fresnel", Float) = 1
        _FresnelEmission("Fresnel Emission", Float) = 1
        [Toggle]_UseCustomData("Use Custom Data?", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }

        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_Mask);
            SAMPLER(sampler_Mask);
            TEXTURE2D(_Noise);
            SAMPLER(sampler_Noise);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Mask_ST;
                float4 _Noise_ST;
                half4 _SpeedMainTexUVNoiseZW;
                half4 _FrontFacesColor;
                half4 _BackFacesColor;
                half4 _FresnelColor;
                half _Emission;
                half _UseFresnel;
                half _SeparateFresnel;
                half _SeparateEmission;
                half _Fresnel;
                half _FresnelEmission;
                half _UseCustomData;
                half _Cutoff;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
                float4 uv4        : TEXCOORD4;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half3 normalWS    : TEXCOORD1;
                float2 uvMain     : TEXCOORD2;
                float2 uvMask     : TEXCOORD3;
                float4 uv4        : TEXCOORD4;
                half4 color       : COLOR;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS = posInputs.positionCS;
                OUT.positionWS = posInputs.positionWS;
                OUT.normalWS = normalize(normalInputs.normalWS);
                OUT.uvMain = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.uvMask = TRANSFORM_TEX(IN.uv, _Mask);
                OUT.uv4.xy = IN.uv4.xy * _Noise_ST.xy + _Noise_ST.zw;
                OUT.uv4.zw = IN.uv4.zw;
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN, half facing : VFACE) : SV_Target
            {
                half3 normalWS = normalize(IN.normalWS);
                half3 viewDirWS = normalize(GetWorldSpaceViewDir(IN.positionWS));

                half NdotV = saturate(dot(normalWS, viewDirWS));
                half fresnel = pow(saturate(1.0h - NdotV), max(_Fresnel, 0.0001h));

                half4 frontBase = lerp(
                    _FrontFacesColor,
                    (_FrontFacesColor * (1.0h - fresnel)) + (_FresnelEmission * _FresnelColor * fresnel),
                    saturate(_UseFresnel)
                );

                half faceMask = facing >= 0 ? 0.0h : 1.0h;
                half4 faceColor = lerp(frontBase, _BackFacesColor, faceMask);

                float2 mainUV = IN.uvMain + (_SpeedMainTexUVNoiseZW.xy * _Time.y);
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, mainUV);

                half4 regularEmission = faceColor * _Emission * IN.color * IN.color.a * mainTex;
                half4 separateEmission = (faceColor + (_FresnelColor * mainTex * _SeparateEmission)) * _Emission * IN.color * IN.color.a;
                half4 finalCol = lerp(regularEmission, separateEmission, saturate(_SeparateFresnel));

                float2 noiseUV = IN.uv4.xy + (_Time.y * _SpeedMainTexUVNoiseZW.zw) + IN.uv4.w;
                half maskTex = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, IN.uvMask).r;
                half noiseTex = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, noiseUV).r;
                half customBlend = lerp(1.0h, IN.uv4.z, saturate(_UseCustomData));
                clip(maskTex * noiseTex * customBlend - _Cutoff);

                return half4(finalCol.rgb, mainTex.a * IN.color.a);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
