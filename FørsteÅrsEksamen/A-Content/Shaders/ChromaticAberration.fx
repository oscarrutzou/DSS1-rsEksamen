#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float aberrationAmount = 0.1; 

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TextureCoordinates;
    float2 distFromCenter = uv - 0.5;

// Stronger aberration near the edges by raising to power 3
    float2 aberrated = aberrationAmount * pow(distFromCenter, float2(3.0, 3.0));

// Apply the chromatic aberration effect
    return float4(
		tex2D(SpriteTextureSampler, uv - aberrated).r,
		tex2D(SpriteTextureSampler, uv).g,
		tex2D(SpriteTextureSampler, uv + aberrated).b,
		1.0
	);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};