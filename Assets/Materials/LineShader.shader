Shader "Unlit/LineShader"
{
	SubShader{ Pass {
	Blend SrcAlpha OneMinusSrcAlpha
	ZWrite Off Cull Off Fog { Mode Off }
	//ZTest Always
} }
}
