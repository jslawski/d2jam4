Shader "Custom/DepthMaskCutout"
{
    Properties
    {
        _MainTex ("Mask Texture (Alpha)", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff Threshold", Range(0,1)) = 0.5
    }

    SubShader
    {
        // Renders early so it sets the depth buffer before normal objects draw
        Tags { "Queue" = "Geometry-1" "RenderType" = "TransparentCutout" }

        ColorMask 0
        ZWrite On

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture's alpha channel
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Discard pixels below the threshold. 
                // Discarded pixels won't write to the depth buffer, letting you see through them!
                clip(col.a - _Cutoff);

                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}
