// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

Shader "Point Cloud/stencilPoint"
{
	Properties
	{
		_PointSize("PointSize", Float) = 3
		_MainTex("Texture", 2D) = "white" {}
		[IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 0
	}
		SubShader
		{
			Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+1" }
			LOD 100
			ZTest Always


			Stencil
			{
				Ref[_StencilRef]
				Comp Equal
			}

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID //Insert	
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 color : COLOR;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
					float size : PSIZE;
					UNITY_VERTEX_OUTPUT_STEREO //Insert
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _PointSize;

				v2f vert(appdata v)
				{
					v2f o;

					UNITY_SETUP_INSTANCE_ID(v); //Insert
					UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.color = v.color;
					o.size = _PointSize;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					//fixed4 col = tex2D(_MainTex, i.uv);
					// apply fog
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); //Insert
					UNITY_APPLY_FOG(i.fogCoord, col);
					return i.color;
				}
				ENDCG
		}
		}
}