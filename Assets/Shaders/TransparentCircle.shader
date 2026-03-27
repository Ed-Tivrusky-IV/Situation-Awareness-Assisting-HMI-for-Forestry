Shader "Unlit/TransparentCircles"
{
    Properties
    {
        _Color ("Base Color", Color) = (0,0,0,1)
        _CircleCount ("Circle Count", Int) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
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
                float4 vertex : SV_POSITION;
            };

            float4 _Color;

            // Fixed maximum number of circles
            #define MAX_CIRCLES 50
            float4 _CircleData[MAX_CIRCLES]; // (x,y = center in UV, z = radius, w = unused)
            int _CircleCount;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float alpha = 1.0;

                // Loop over circles
                [loop]
                for (int c = 0; c < _CircleCount; c++) {
                    float2 center = _CircleData[c].xy;
                    float radius = _CircleData[c].z;

                    float dist = distance(i.uv, center);
                    if (dist < radius) {
                        alpha = 0.0; // transparent
                        break; // stop if inside any circle
                    }
                }

                return float4(_Color.rgb, alpha);
            }
            ENDCG
        }
    }
}
