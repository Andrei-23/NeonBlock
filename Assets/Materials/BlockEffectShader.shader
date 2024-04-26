Shader "Custom/BlockEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Hue("Hue", Range(0,360)) = 0
        [Toggle] _ShiftHue("Shift Hue", Float) = 0

        // required for UI.Mask
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off ZTest Always

        Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
 		ColorMask [_ColorMask]

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
                // fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                // fixed4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // o.color = v.color;
                return o;
            }
            

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Hue;
            float _ShiftHue;

            fixed3 HSVToRGB(fixed3 c)
	        {
		        float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
		        fixed3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
		        return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
	        }
    
	        fixed3 RGBToHSV(fixed3 c)
	        {
		        float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
		        float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
		        float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
		        float d = q.x - min( q.w, q.y );
		        float e = 1.0e-10;
		        return fixed3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
	        }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed3 hsv = RGBToHSV(col.rgb.xyz);

                if(_ShiftHue == 0.0){
                    hsv.x = _Hue / 360.0;
                }
                else{
                    hsv.x += _Hue / 360.0;
                    if(hsv.x > 1.0){
                        hsv.x -= 1.0;
                    }
                }
                
                col.rgb.xyz = HSVToRGB(hsv);
                return col;
            }
            ENDCG
        }
    }
}
