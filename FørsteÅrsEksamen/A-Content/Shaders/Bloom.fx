#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
int size = 5;
float separation = 3;
float threshold = 0.25; // 0.25 is pretty cool
float amount = 1;


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
    float4 tex = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    // Calculate the grayscale value (average of RGB channels)
    //float grayscale = dot(tex.rgb, float3(0.3333, 0.3333, 0.3333));

    //// Check if the grayscale value is above the threshold
    //float4 result = (grayscale > threshold) ? float4(1, 1, 1, 1) : float4(0, 0, 0, 1);

    return tex * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};