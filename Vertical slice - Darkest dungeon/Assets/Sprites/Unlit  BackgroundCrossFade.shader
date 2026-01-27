Shader "Unlit/BackgroundCrossFade"
{
    Properties
    {
        _MainTex ("Sharp Texture", 2D) = "white" {}
        _BlurTex ("Blur Texture", 2D) = "white" {}
        _Blend ("Blend", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
          "RenderType"="Opaque"
         "Queue"="Geometry"
        }


        Pass
        {
           
            Blend SrcAlpha OneMinusSrcAlpha

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

            sampler2D _MainTex;
            sampler2D _BlurTex;
            float _Blend;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 sharp = tex2D(_MainTex, i.uv);
                fixed4 blur  = tex2D(_BlurTex, i.uv);

                
                fixed4 col = lerp(sharp, blur, _Blend);

                
                col.a = 1.0;

                return col;
            }
            ENDCG
        }
    }
}
