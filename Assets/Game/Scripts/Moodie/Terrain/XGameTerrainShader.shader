Shader "My/XGameTerrainShader" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_SecondTex("Second (RGB)",2D) = "white"{}
		_ThirdTex("ThirdTex (RGB)",2D) = "white"{}
		_FourthTex("FourthTex (RGB)",2D) = "white"{}
		_Mask("Mask(RG)",2D) = "white"{}
		//_Color("Color",Color) = (1,1,1,1)
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
		#pragma surface surf Lambert fullforwardshadows
		sampler2D _MainTex;
		sampler2D _SecondTex;
		sampler2D _ThirdTex;
		sampler2D _FourthTex;
		sampler2D _Mask;

		struct Input {
			float2 uv_MainTex;
			float2 uv_Mask;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			half4 c1 = tex2D(_MainTex, IN.uv_MainTex);
			half4 c2 = tex2D(_SecondTex, IN.uv_MainTex);
			half4 c3 = tex2D(_ThirdTex, IN.uv_MainTex);
			half4 c4 = tex2D(_FourthTex, IN.uv_MainTex);
			half4 cm = tex2D(_Mask, IN.uv_Mask);
			o.Albedo = c1.rgb*cm.r + c2.rgb*cm.g + c3.rgb*cm.b + c4.rgb*cm.a;
			o.Alpha = c1.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}