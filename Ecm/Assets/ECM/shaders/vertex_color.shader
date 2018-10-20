Shader "Custom/vertex_color" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex1 ("Albedo1 (RGB)", 2D) = "white" {}
		_MainTex2("Albedo2 (RGB)", 2D) = "white" {}
		_MainTex3("Albedo3 (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		struct Input {
			float2 uv_MainTex;
			float3 vertexColor; // Vertex color stored here by vert() method
		};

		struct v2f {
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
		}

		sampler2D _MainTex1;
		sampler2D _MainTex2;
		sampler2D _MainTex3;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 tex = lerp(tex2D(_MainTex1, IN.uv_MainTex), lerp(tex2D(_MainTex2, IN.uv_MainTex), tex2D(_MainTex3, IN.uv_MainTex), IN.vertexColor.x), IN.vertexColor.y);
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = tex.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			//o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
