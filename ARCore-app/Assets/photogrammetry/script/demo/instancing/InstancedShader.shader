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
                StructuredBuffer<float4> positionBuffer;
                Texture2D<float4> depthTexture;
                float width;
            #endif

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv_MainTex : TEXCOORD0;
                    float4 color : TEXCOORD3;
                    SHADOW_COORDS(4)
                };

                void rotate2D(inout float2 v, float r)
                {
                    float s, c;
                    sincos(r, s, c);
                    v = float2(v.x * c - v.y * s, v.x * s + v.y * c);
                }

                v2f vert(appdata_full v, uint instanceID : SV_InstanceID)
                {
                #if SHADER_TARGET >= 45
                    float4 data = positionBuffer[instanceID];

                    int x = int(instanceID) % width;
                    int y = int(instanceID) / width;

                    float depth = depthTexture.Load(int3(x, y, 0)).w;
                    float3 color = depthTexture.Load(int3(x, y, 0)).xyz;
                #else
                    float4 data = 0;
                    float depth = 0;
                #endif

                    data.z += Linear01Depth(depth) * _ProjectionParams.z;
                    float3 localPosition = v.vertex.xyz * data.w;
                    float3 worldPosition = data.xyz + localPosition;
                    float3 worldNormal = v.normal;

                    v2f o;
                    o.pos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
                    o.uv_MainTex = v.texcoord;
                    o.color = float4(color, depth);
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