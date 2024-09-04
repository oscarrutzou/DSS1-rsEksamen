#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float innerRadius; 
float outerRadius; 

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
    float2 center = float2(0.5, 0.5); // Center of the screen (normalized coordinates)
    float2 delta = abs(uv - center); // Distance from the center

// Calculate the distance from the center (normalized to [0, 1])
    float distance = length(delta);

// Smoothly interpolate the vignette intensity based on distance
    float vig = smoothstep(outerRadius, innerRadius, distance);

    float4 color = float4(1, 1, 1, 1);
// Apply the vignette effect
    return tex2D(SpriteTextureSampler, uv) * color * vig;
}


technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};