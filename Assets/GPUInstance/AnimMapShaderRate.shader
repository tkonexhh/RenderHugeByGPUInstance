﻿Shader "XHH/AnimMapShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _AnimMap ("AnimMap", 2D) = "white" { }
        _AnimRate ("Anim Rate", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Cull off

        Pass
        {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            //开启gpu instancing
            #pragma multi_compile_instancing


            #include "UnityCG.cginc"

            struct appdata
            {
                float2 uv: TEXCOORD0;
                float4 pos: POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv: TEXCOORD0;
                float4 vertex: SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _AnimMap;
            float4 _AnimMap_TexelSize;//x == 1/width


            UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
            UNITY_DEFINE_INSTANCED_PROP(float, _AnimRate)
            UNITY_INSTANCING_BUFFER_END(UnityPerMaterial);
            // float _AnimRate;
            
            v2f vert(appdata v, uint vid: SV_VertexID)//vid对应的就是
            {
                UNITY_SETUP_INSTANCE_ID(v);

                // float f = _AnimRate;//_AnimRate/_ani / _AnimLen;
                float f = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _AnimRate);

                fmod(f, 1.0);

                float animMap_x = (vid + 0.5) * _AnimMap_TexelSize.x;
                float animMap_y = f;

                float4 pos = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));

                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex = UnityObjectToClipPos(pos);
                return o;
            }
            
            fixed4 frag(v2f i): SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
            
        }
    }
}
