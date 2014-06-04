Shader "Custom/UnlitTeamColor" {
	Properties
	{
		_MainTex ("Base (RGBA)", 2D) = "white" {}
		_TeamColor ("Team Color (RGBA)", Color) = (0.0, 0.0, 0.0, 1.0)
	}
	
	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
			"LightMode" = "Vertex"
			"ForceNoShadowCasting" = "True"
			"IgnoreProjector" = "True"
		}
		
		Cull Back
		Lighting Off
		Fog { Mode Off }
		ZWrite On
		ZTest On
		AlphaTest Off
		ColorMask RGB
		Blend Off

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"
	
				struct appdata_t
				{
					fixed4 vertex : POSITION;
					fixed2 texcoord : TEXCOORD0;
				};
	
				struct v2f
				{
					fixed4 vertex : SV_POSITION;
					fixed2 texcoord : TEXCOORD0;
				};
	
				sampler2D _MainTex;
				fixed4 _TeamColor;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = v.texcoord;								
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
					return lerp(_TeamColor, col, col.a);
				}
			ENDCG
		}
	}
}
