Shader "Custom/SemiTransparentDuplicate"
{
    Properties
    {
        _Color("Color",Color) = (0,0,1,0.1)
    }
    SubShader
    {
Tags {"Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent"}
        ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off
		LOD 100

        Pass
        {
		ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off

        LOD 100
            Stencil {
                Ref 0
                Comp Equal
				ReadMask 2
				WriteMask 2
                Pass Invert 
                Fail Keep 
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;
                return col;
            }
            ENDCG
        }
    }
}