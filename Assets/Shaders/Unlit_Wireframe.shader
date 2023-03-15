Shader "Unlit/Wireframe"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WireframeColour("Wireframe colour", color) = (1.0, 1.0, 1.0, 1.0)
        _WireframeWidth("Wireframe Width", float) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 barycentric : TEXCOORD2;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 barycentric : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.barycentric = v.barycentric;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 _WireframeColour;
            float _WireframeWidth;
            fixed4 frag(v2f i) : SV_Target
            {
                float3 barycentric = i.barycentric;
                
                const float3 coord_scale = fwidth(barycentric);
                barycentric /= coord_scale;
                const float dist = min(barycentric.x, min(barycentric.y, barycentric.z));
                const float wireframe = 1-smoothstep(0, _WireframeWidth, dist);
                fixed4 col = tex2D(_MainTex, i.uv);
                col = lerp(col, _WireframeColour, wireframe);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}