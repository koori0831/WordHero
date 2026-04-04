Shader "Hovl/Particles/Ice"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _Color("Color", Color) = (0.02352941,0.2055747,1,1)
        _UpColor("Up Color", Color) = (0.4575472,0.7381514,1,1)
        _ColorPosition("Color Position", Range(0, 1)) = 0.35
        _Emission("Emission", Float) = 1
        [HDR]_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        _FresnelPower("Fresnel Power", Float) = 6
        _FresnelScale("Fresnel Scale", Float) = 1
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

        Cull Back
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                half4 _UpColor;
                half _ColorPosition;
                half _Emission;
                half4 _FresnelColor;
                half _FresnelPower;
                half _FresnelScale;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                half4 color       : COLOR;
                float3 positionWS : TEXCOORD1;
                half3 normalWS    : TEXCOORD2;
                half3 normalOS    : TEXCOORD3;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS = positionInputs.positionCS;
                OUT.positionWS = positionInputs.positionWS;
                OUT.normalWS = normalize(normalInputs.normalWS);
                OUT.normalOS = normalize(IN.normalOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                half remappedColorPos = lerp(-1.0h, 1.0h, _ColorPosition);
                half verticalMask = saturate(IN.normalOS.y + remappedColorPos);
                half4 baseTint = lerp(_Color, _UpColor, verticalMask);

                half3 normalWS = normalize(IN.normalWS);
                half3 viewDirWS = normalize(GetWorldSpaceViewDir(IN.positionWS));
                half fresnel = saturate(_FresnelScale * pow(1.0h - saturate(dot(normalWS, viewDirWS)), _FresnelPower));

                half4 baseCol = mainTex * baseTint * (1.0h - fresnel);
                half4 fresnelCol = fresnel * _FresnelColor;
                half4 finalCol = (baseCol + fresnelCol) * IN.color;

                half3 emission = finalCol.rgb * _Emission;
                return half4(finalCol.rgb + emission, finalCol.a);
            }
            ENDHLSL
        }
    }

    FallBack Off
}