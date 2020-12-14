Shader "Unlit/blockingStencilShader"
{
	Properties
	{
		_MyColor("Screen Color", Color) = (103,99,173,1)
		_MainTex("Texture", 2D) = "white" {}
	[IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 0
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry-3" }
		LOD 100


		ZWrite On
		Blend Zero One

		Pass{
		Stencil{
		Ref 1
		Comp Always
		Pass Replace
	}
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
		float2 uv : TEXCOORD0;
		float4 color : COLOR;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		float4 color : COLOR;
	};

	sampler2D _MainTex;
	float4 _MyColor;
	float4 _MainTex_ST;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.color = _MyColor;
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture
		float4 col = tex2D(_MainTex, i.uv);
		// apply fog
		UNITY_APPLY_FOG(i.fogCoord, col);
		return i.color;
	}
		ENDCG
	}
	}
}