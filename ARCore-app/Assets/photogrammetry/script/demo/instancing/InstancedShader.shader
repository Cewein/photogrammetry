Shader "Instanced/InstancedShader" {
    Properties{
        _MainTex("Albedo (RGB)", 2D) = "white" {}
    }
        SubShader{

            Pass {

                Tags {"LightMode" = "ForwardBase"}

                CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
                #pragma target 4.5

                #include "UnityCG.cginc"
                #include "UnityLightingCommon.cginc"
                #include "AutoLight.cginc"

                sampler2D _MainTex;

            #if SHADER_TARGET >= 45
                Texture2D<float4> depthTexture;
                int imageWidth;
                int imageHeight;
                float3 cameraPosition;
                float3 cameraForward;
                float3 cameraUp;
                float3 vfov;

            #endif

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv_MainTex : TEXCOORD0;
                    float4 color : TEXCOORD3;
                    SHADOW_COORDS(4)
                };

                v2f vert(appdata_full v, uint instanceID : SV_InstanceID)
                {
                #if SHADER_TARGET >= 45
                    float x = float(instanceID) % imageWidth;
                    float y = float(instanceID) / imageWidth;
                    
                    float4 depthTex = depthTexture.Load(int3(x, y, 0)); 
                    float depth = depthTex.w;
                    float3 color = depthTex.xyz;

                #else
                    float4 data = 0;
                    float depth = 0;
                #endif

                    float2 uv = float2(x,y) / float2(imageWidth, imageHeight);

                    float3 localPosition = v.vertex.xyz * 1.0/ imageWidth;
                    float3 worldPosition = float3(uv, Linear01Depth(depth) * _ProjectionParams.z) + localPosition;
                    float3 worldNormal = v.normal; 

                    v2f o;
                    o.pos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
                    o.uv_MainTex = v.texcoord;
                    o.color = float4(color, 1.0);
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    float4 colAndPos = i.color;
                    return colAndPos;
                }

                ENDCG
            }
    }
}