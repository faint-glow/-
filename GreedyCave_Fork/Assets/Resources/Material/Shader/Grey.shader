Shader "Hidden/Grey"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        //Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                col.rgb = dot(col.rgb, float3(0.299, 0.587, 0.144));//灰度公式
                // just invert the colors
                //col.rgb = 1 - col.rgb;
                if (col.a < 0.1) //Blend SrcAlpha OneMinusSrcAlpha一起使用才可以让透明的不显示
                {
                    return float4(0, 0, 0, 0);
                }

                return col;
            }
            ENDCG
        }
    }
}
