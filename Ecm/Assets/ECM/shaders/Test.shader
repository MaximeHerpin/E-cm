Shader "Custom/CharacterOutline" {
	Properties{
		[PerRendererData]_Color("Color", Color) = (1,1,1,1)
		//[PerRendererData]_Color("Color", Color) = (1,1,1,1)
		//_Color ("Color", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
	}
	SubShader{

		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		fixed4 _Color;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color;
			o.Metallic = 0;
			o.Smoothness = _Glossiness;
		}
		ENDCG	
	}
	FallBack "Diffuse"
}
