Shader "TreeShader" {
	Properties{
	   _Alpha("Alpha", Range(0,1)) = 0.3
	   _MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
	   _CutOff("Cut off", Range(0,1)) = 0.1
	   _Position("Player Position", Vector) = (0, 0, 0, 0)
	   _MaxDistance("Max Distance To Clip Transperncy", Float) = 5
	   _MinDistance("Min Distance", Float) = 0
	   _TransparentFlag("Flag for transparency", Float) = 0
	}
		SubShader{
		   Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		   Blend SrcAlpha OneMinusSrcAlpha
		   AlphaTest Greater 0.1
	 /*      Stencil {
		     Ref 1
			 Comp NotEqual
		   }*/

		   Pass {
			  CGPROGRAM

			  #pragma vertex vert
			  #pragma fragment frag

			  // User-specified uniforms
			  uniform sampler2D _MainTex;
			  uniform float _CutOff;
			  uniform float _Alpha;
			  uniform float4 _Position;
			  uniform float _MaxDistance;
			  uniform float _MinDistance;
			  uniform float _TransparentFlag;

			  struct vertexInput {
				 float4 vertex : POSITION;
				 float4 tex : TEXCOORD0;
			  };
			  struct vertexOutput {
				 float4 pos : SV_POSITION;
				 float4 tex : TEXCOORD0;
				 float4 worldPos : TEXCOORD1;
			  };

			  vertexOutput vert(vertexInput input)
			  {
				 vertexOutput output;
				 output.worldPos = mul(unity_ObjectToWorld, input.vertex);
				 output.pos = UnityObjectToClipPos(input.vertex);
				 output.tex = input.tex;
				 return output;
			  }

			  float4 frag(vertexOutput input) : COLOR
			  {
				 float dist = distance(input.worldPos.xyz, _Position.xyz);
				 dist = clamp(dist, _MinDistance, _MaxDistance);
				 dist = (dist - _MinDistance) / (_MaxDistance - _MinDistance);

				 float4 color = tex2D(_MainTex, float2(input.tex.xy));
				 if (color.a < _CutOff) discard;

				 color.a = max(dist, _TransparentFlag);

				 return color;
			   }

			   ENDCG
			}
	   }
}