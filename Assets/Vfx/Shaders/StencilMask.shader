Shader "Custom/StencilMask"
{
    Properties
    {
        _StencilRef ("Stencil Ref ID", Int) = 1
    }

    SubShader
    {
        Tags { "Queue" = "Geometry-1" "RenderType" = "Opaque" }

        // Turn off color rendering
        ColorMask 0
        ZWrite Off

        // Write the Ref ID to the stencil buffer
        Stencil
        {
            Ref [_StencilRef]
            Comp Always
            Pass Replace
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 vertex : SV_POSITION; };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target { return fixed4(0,0,0,0); }
            ENDCG
        }
    }
}
