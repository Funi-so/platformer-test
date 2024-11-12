Shader "Custom/SpinningTexture"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Slider("Spin", Range(0,10)) = 0.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline"}
        Pass {
            Tags {"LightMode"="UniversalForward"}
            HLSLPROGRAM


            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f{
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            sampler2D _MainTex;
            float _Slider;
            float4 _MainTex_ST;
            v2f vert (appdata v) {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            float4 frag (v2f i) : SV_Target {
                float t = _Time.y * _Slider *  3.14 / 100.0;
                float2x2 m = float2x2(
                    cos(t), -sin(t),
                    sin(t), cos(t)
                );
                /*float2x2 m = float2x2(
                    1,0,
                    0,1
                );*/

                float2 c = (0.5, 0.5);
                float2 uv = mul(m, i.uv - c) + c;

                float4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
