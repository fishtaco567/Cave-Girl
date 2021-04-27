Shader "Unlit/DrawScren"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ShadowTex("ShadowTex", 2D) = "black" {}
		_ShadowCol("Shadow Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"PreviewType" = "Plane"
		}
        LOD 100

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

			static const float PI = 3.14159265f;

			v2f vert(appdata_t IN, out float4 outpos : SV_POSITION)
			{
				v2f OUT;
				outpos = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _ShadowTex;
			fixed4 _ShadowCol;

			fixed4 frag(v2f IN, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{
				fixed4 shadow = tex2D(_ShadowTex, IN.texcoord);
				
				fixed4 shadowC = saturate(_ShadowCol + shadow.rgbr);

				fixed4 c = tex2D(_MainTex, IN.texcoord) * shadowC;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
    }
}
