Shader "Hovl/Particles/Distortion"
{
    Properties
    {
        _NormalMap("Normal Map", 2D) = "bump" {}
        _Distortionpower("Distortion power", Float) = 0.05
        _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _NormalMap_ST;
                half _Distortionpower;
                half _InvFade;
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
                float2 normalUV : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float4 positionNDC : TEXCOORD2;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = pos.positionCS;
                OUT.screenPos = ComputeScreenPos(pos.positionCS);
                OUT.positionNDC = pos.positionNDC;
                OUT.color = IN.color;
                OUT.normalUV = TRANSFORM_TEX(IN.uv.xy, _NormalMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

                #if UNITY_REVERSED_Z
                    real rawDepth = SampleSceneDepth(screenUV);
                #else
                    real rawDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(screenUV));
                #endif
                float sceneEye = LinearEyeDepth(rawDepth, _ZBufferParams);
                float partEye = LinearEyeDepth(IN.screenPos.z / IN.screenPos.w, _ZBufferParams);
                float fade = saturate(_InvFade * (sceneEye - partEye));

                half3 nrm = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.normalUV));
                half2 distortion = nrm.rg;
                half alphaMask = saturate((abs(nrm.r) + abs(nrm.g) * 30.0h) - 0.03h);

                float2 opaqueTexel = _ScreenSize.zw - 1.0;
                float2 offset = distortion * opaqueTexel * _Distortionpower * (IN.color.a * fade);
                float2 distortedUV = screenUV + offset;

                half4 col = half4(SampleSceneColor(distortedUV), 1.0);
                col *= IN.color;
                col.a = saturate(col.a * alphaMask);
                return col;
            }
            ENDHLSL
        }
    }

    FallBack Off
}
