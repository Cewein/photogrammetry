Shader "Instanced/PointCloudShader"
{
    Properties
    {
    }
    SubShader
    {
        Pass
        {
            Tags {"RenderType" = "Transparent"}
            ZWrite Off
            Lighting Off
            Fog { Mode Off }

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
                float4 pos  : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            Texture2D<float4> DepthTexture;
            int width;
            int height;

            float3 cameraPosition;
            float3 cameraForward;
            float3 cameraUp;
            float3 cameraRight;
            float cameraFOV;

            v2f vert(appdata_full v, uint instanceID : SV_InstanceID)
            {
                int x = instanceID % width;
                int y = instanceID / width;

                x = clamp(x, 0, width);
                y = clamp(y, 0, height);

                float2 uv = float2(x,y) / float2(width,height);

                float4 data = DepthTexture.Load(int3(x,y,0));

                float3 target = cameraPosition + cameraForward * float3(2.0, 2.0, 2.0);
                float dir = normalize(uv.x * cameraRight + uv.y * cameraUp + 1.5 * cameraForward);

                float3 worldPosition = cameraPosition + dir*data.w;


                v2f o;
                o.pos = float4(worldPosition, 1.0f);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = fixed4(1.0,0.0,0.0,1.0);
                return col;
            }
            ENDCG
        }
    }
}
