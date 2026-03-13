Shader "Custom/URP_RegionFog"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1,1,1,1)

        _FogColorA("Fog Color A", Color) = (0.5,0.6,0.7,1)
        _FogColorB("Fog Color B", Color) = (0.2,0.2,0.3,1)

        _FogDensityA("Fog Density A", Float) = 0.02
        _FogDensityB("Fog Density B", Float) = 0.05
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode"="UniversalForward"}

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseColor;
            float4 _BaseMap_ST;

            float4 _FogColorA;
            float4 _FogColorB;

            float _FogDensityA;
            float _FogDensityB;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);

                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);

                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 normal = normalize(IN.normalWS);

                Light mainLight = GetMainLight();

                float NdotL = saturate(dot(normal, mainLight.direction));

                float3 albedo =
                    SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv).rgb
                    * _BaseColor.rgb;

                float3 lighting = albedo * mainLight.color * NdotL;

                float3 color = lighting;

                // ---------- «¯”Ú≈–∂œ ----------
                float fogDensity;
                float3 fogColor;

                if (IN.positionWS.x < 0)
                {
                    fogDensity = _FogDensityA;
                    fogColor = _FogColorA.rgb;
                }
                else
                {
                    fogDensity = _FogDensityB;
                    fogColor = _FogColorB.rgb;
                }

                // ---------- æ‡¿ÎŒÌ ----------
                float dist = distance(_WorldSpaceCameraPos, IN.positionWS);

                float fogFactor = 1 - exp(-fogDensity * dist);

                color = lerp(color, fogColor, saturate(fogFactor));

                return float4(color,1);
            }

            ENDHLSL
        }
    }
}