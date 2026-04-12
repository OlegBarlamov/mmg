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
float4x4 WorldInverse;

float3 CameraPositionWS;
float Brightness;
float FalloffSharpness;

float2 SectorUVOffset;
float SectorUVScale;
float EdgeFadeStart;

#define MAX_STEPS 48

texture GalaxyTexture;
sampler2D GalaxyTextureSampler = sampler_state
{
	Texture = <GalaxyTexture>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float3 ObjectPos : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.ObjectPos = input.Position.xyz;

	return output;
}

float2 IntersectBox(float3 rayOrigin, float3 rayDir)
{
	float3 boxMin = float3(-0.5, -0.5, -0.5);
	float3 boxMax = float3(0.5, 0.5, 0.5);

	float3 invDir = 1.0 / rayDir;
	float3 t0 = (boxMin - rayOrigin) * invDir;
	float3 t1 = (boxMax - rayOrigin) * invDir;

	float3 tmin = min(t0, t1);
	float3 tmax = max(t0, t1);

	float tNear = max(max(tmin.x, tmin.y), tmin.z);
	float tFar = min(min(tmax.x, tmax.y), tmax.z);

	return float2(tNear, tFar);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float3 camOS = mul(float4(CameraPositionWS, 1.0), WorldInverse).xyz;
	float3 rayDir = normalize(input.ObjectPos - camOS);

	float2 tRange = IntersectBox(camOS, rayDir);
	float tNear = max(tRange.x, 0.0);
	float tFar = tRange.y;

	if (tFar <= tNear)
		discard;

	float stepSize = (tFar - tNear) / MAX_STEPS;

	float3 accColor = float3(0, 0, 0);
	float accAlpha = 0;

	for (int i = 0; i < MAX_STEPS; i++)
	{
		float t = tNear + (i + 0.5) * stepSize;
		float3 P = camOS + rayDir * t;

		float2 uv = P.xz * SectorUVScale + SectorUVOffset;
		float4 texSample = tex2Dlod(GalaxyTextureSampler, float4(uv, 0, 0));

		float verticalFalloff = exp(-P.y * P.y * FalloffSharpness);

		float galaxyDist = length(uv - 0.5);
		float edgeFade = 1.0 - smoothstep(EdgeFadeStart, 0.5, galaxyDist);

		float density = texSample.a * verticalFalloff * edgeFade * stepSize * Brightness;

		accColor += texSample.rgb * density * (1.0 - accAlpha);
		accAlpha += density * (1.0 - accAlpha);
	}

	return float4(accColor, saturate(accAlpha));
}

technique GalaxySlabDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
