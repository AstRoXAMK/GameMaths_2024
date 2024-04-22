Shader "Unlit/TerrainShader"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Texture1 ("Texture1", 2D) = "white" {}
        _Texture2 ("Texture2", 2D) = "white" {}
        _Texture3 ("Texture3", 2D) = "white" {}
        _SplatMap ("SplatMap", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _Texture1;
            sampler2D _Texture2;
            sampler2D _Texture3;
            sampler2D _SplatMap;
            float4 _MainTex_ST;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // sample the splat map
                fixed4 splat = tex2D(_SplatMap, i.uv);
                // sample the textures
                fixed4 col1 = tex2D(_Texture1, i.uv);
                fixed4 col2 = tex2D(_Texture2, i.uv);
                fixed4 col3 = tex2D(_Texture3, i.uv);
                // blend the textures
                fixed4 col = splat.r * col1 + splat.g * col2 + splat.b * col3;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
