Shader "Custom/MobileShader"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _BumpMap ("Normal Map", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _BaseMap;
            float4 _BaseColor;
            float _Metallic;
            float _Smoothness;
            sampler2D _BumpMap;

            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldTangent : TEXCOORD2;
                float3 worldBitangent : TEXCOORD3;
                float2 uv : TEXCOORD0;
            };

            VertexOutput vert (VertexInput v)
            {
                VertexOutput o;

                // Transform vertex position to clip space
                o.position = UnityObjectToClipPos(v.vertex);

                // Transform normal, tangent, and bitangent to world space
                o.worldNormal = normalize(mul(v.normal, (float3x3)unity_ObjectToWorld));
                o.worldTangent = normalize(mul(v.tangent.xyz, (float3x3)unity_ObjectToWorld));
                o.worldBitangent = cross(o.worldNormal, o.worldTangent) * v.tangent.w;

                // Pass UV coordinates
                o.uv = v.uv;

                return o;
            }

            fixed4 frag (VertexOutput i) : SV_Target
            {
                // Sample the base texture
                fixed4 albedo = tex2D(_BaseMap, i.uv) * _BaseColor;

                // Compute normal from normal map
                float3 normalTS = UnpackNormal(tex2D(_BumpMap, i.uv));
                float3 worldNormal = normalize(
                    normalTS.x * i.worldTangent +
                    normalTS.y * i.worldBitangent +
                    normalTS.z * i.worldNormal
                );

                // Simple directional light
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(dot(worldNormal, lightDir), 0.0);
                float3 diffuse = albedo.rgb * NdotL;

                // Specular highlights
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.position.xyz);
                float3 halfDir = normalize(lightDir + viewDir);
                float NdotH = max(dot(worldNormal, halfDir), 0.0);
                float3 specular = pow(NdotH, 1.0 / _Smoothness) * _Metallic;

                return float4(diffuse + specular, albedo.a);
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
