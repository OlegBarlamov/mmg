#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;

float4 StarColor;
float Intensity;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 uv = input.TexCoord - 0.5;
	float dist = length(uv);

	float coreGlow = exp(-dist * 12.0) * Intensity;

	float angle = atan2(uv.y, uv.x);

	float rays4 = pow(abs(cos(angle * 2.0)), 80.0) * exp(-dist * 4.0) * 0.8;
	float rays6 = pow(abs(cos(angle * 3.0 + 0.5)), 120.0) * exp(-dist * 3.0) * 0.4;

	float outerGlow = exp(-dist * 5.0) * 0.3 * Intensity;

	float brightness = coreGlow + rays4 + rays6 + outerGlow;
	float alpha = saturate(brightness);

	return float4(StarColor.rgb * brightness, alpha);
}

technique StarBeamDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
