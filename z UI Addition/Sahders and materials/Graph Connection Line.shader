Shader "Unlit/Graph Connection Line" {
	Properties{
		_MainTex("Laser Texture", 2D) = "black" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		//_Speed("Time Speed", Range(0.5, 20)) = 1
		//_Strength("Color Strength", Range(0.5, 2)) = 1
	}
	SubShader {
		Tags{ "RenderType" = "Transparent"
			"Queue" = "Transparent" }
		Pass {

			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Off
			Zwrite Off

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				uniform sampler2D _MainTex;
				uniform fixed4 _Color;
				//uniform half _Strength;
				//uniform half _Speed;

				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					fixed4 colorSource : COLOR0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
					fixed4 colorSource : COLOR0;
				};


				v2f vert(appdata v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.colorSource = v.colorSource;
					return o;
				}

				fixed4 frag(v2f i) : COLOR{
					fixed4 laser = mul(unity_ObjectToWorld, tex2D(_MainTex, i.uv));
					//+half2(-_Time.y * _Speed, 0)));
					fixed atten = pow(i.colorSource.a, 0.5);
					return (laser.r * _Color) * _Color.a * atten;
					//return (laser.r * _Color * _Strength) * _Color.a * atten;
				}
			ENDCG
		}
	}
}
