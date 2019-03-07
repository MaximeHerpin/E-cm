Shader "Mtree/Unlit/Normal"
    {
        Properties
        {
			_MainTex("Albedo (RGB)", 2D) = "white" {}
			_Clip("Clip", Range(0,1)) = 0.5
        }
     
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
     
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
					float3 normal : NORMAL;
				};

                struct v2f
                {
                    float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 norm : TEXCOORD1;
                };
     
				float4 _MainTex_ST;
				sampler2D _MainTex;
				half _Clip;
     
                v2f vert (appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    float3 worldNorm = UnityObjectToWorldNormal(v.normal);
                    float3 viewNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);               
					o.norm = viewNorm;
                    return o;
                }
				

                fixed4 frag (v2f i) : SV_Target
                {
					fixed a = tex2D(_MainTex, i.uv).a;
					fixed4 col = float4(1,1,1,1);
                    col.rgb = i.norm *0.5 + 0.5;
					clip(a - _Clip);
                    return col;
                }
     
                ENDCG
            }
        }

    }
