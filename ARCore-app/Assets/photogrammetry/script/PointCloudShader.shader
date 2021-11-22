Shader "Unlit/PointCloudShader"
{
    Properties
    {
    }
    SubShader
    {
        Pass
        {
            Tags {"Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            ZWrite Off
            Lighting Off
            Fog { Mode Off }

            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #pragma target 4.5

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            Texture2D<float4> DepthTexture;

            v2f vert(appdata_full v, uint instanceID : SV_InstanceID)
            {
                float4 data = positionBuffer[instanceID];

                float4 color = DepthTexture.Load(int3(id.xy, 0));

                float3 localPosition = v.vertex.xyz * data.w;
                float3 worldPosition = data.xyz + localPosition;


                v2f o;
                o.pos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
