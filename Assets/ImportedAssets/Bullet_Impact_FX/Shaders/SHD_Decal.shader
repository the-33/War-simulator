Shader "URP/Custom/DecalLit"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _ColorTint("Color Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "DecalProjector"="True" }
        LOD 200
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Offset -20, -20

        Pass
        {
            Name "Decal"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _BumpMap;
            float4 _ColorTint;
            float _Glossiness;
            float _Metallic;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 baseCol = tex2D(_MainTex, IN.uv) * _ColorTint;
                float3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv));

                // Output as a standard URP decal fragment
                half4 outColor;
                outColor.rgb = baseCol.rgb;
                outColor.a = baseCol.a;

                // Output to decal system (these will be combined by Unity's decal renderer)
                SurfaceData surfaceData;
                surfaceData.albedo = outColor.rgb;
                surfaceData.normalTS = normal;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Glossiness;
                surfaceData.alpha = outColor.a;

                return outColor;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
