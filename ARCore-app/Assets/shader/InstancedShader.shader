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

                Texture2D<float4> depthTexture;
                int imageWidth;
                int imageHeight;
                float hfov;
                float meshSize;
                float4x4 invKmat;

                float4x4 camMat;
                float4x4 InvCamMatProj;
                float3 camPos;


                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv_MainTex : TEXCOORD0;
                    float4 color : TEXCOORD3;
                    SHADOW_COORDS(4)
                };

                float3 getWorldPos(float x, float y, float depth)
                {
                    float2 uv = float2(x, y) / float2(imageWidth, imageHeight);

                    float zDepth = Linear01Depth(depth) * _ProjectionParams.z;

                    float3 viewfloattor = mul(InvCamMatProj, float4(uv * 2.0 - 1.0, 0.0, -1.0));
                    viewfloattor = mul(camMat, float4(viewfloattor, 0.0));

                    float3 rayOrigin = camPos;
                    float viewLength = length(viewfloattor);
                    float3 rayDir = viewfloattor / viewLength;

                    return rayOrigin + (rayDir * zDepth);
                }

                v2f vert(appdata_full v, uint instanceID : SV_InstanceID)
                {
                    float x = float(instanceID) % imageWidth;
                    float y = float(instanceID) / imageWidth;
                    
                    float4 depthTex = depthTexture.Load(int3(x, y, 0)); 
                    float depth = depthTex.w;
                    float3 color = depthTex.xyz;

                    float3 localPosition = v.vertex.xyz * meshSize;
                    float3 worldPosition = getWorldPos(x, y, depth) + localPosition;
                    float3 worldNormal = v.normal; 

                    v2f o;
                    o.pos = UnityObjectToClipPos(float4(worldPosition, 1.0f));
                    o.uv_MainTex = v.texcoord;
                    o.color = float4(color, 1.0);
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    return i.color;
                }

                ENDCG
            }
    }
}