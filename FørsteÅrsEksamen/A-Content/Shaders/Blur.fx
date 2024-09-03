#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

texture Texture;
float weights[15];
float offsets[15];

sampler2D TextureSampler = sampler_state
{
    texture = <Texture>;
    minfilter = point;
    magfilter = point;
    mipfilter = point;
};

float4 BlurHorizontal(float4 position : SV_POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_Target
{
    float4 output = float4(0, 0, 0, 1);
    for (int i = 0; i < 15; i++)
    {
        float2 clampedTexCoord = clamp(texCoord + float2(offsets[i], 0), 0.0, 1.0);
        output += tex2D(TextureSampler, clampedTexCoord) * weights[i];
    }
    return output;
}

float4 BlurVertical(float4 position : SV_POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_Target
{
    float4 output = float4(0, 0, 0, 1);
    for (int i = 0; i < 15; i++)
    {
        float2 clampedTexCoord = clamp(texCoord + float2(0, offsets[i]), 0.0, 1.0);
        output += tex2D(TextureSampler, clampedTexCoord) * weights[i];
    }
    return output;
}

float4 PassThroughVS(float4 position : POSITION0) : SV_POSITION
{
    return position;
}

technique Vertical
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL BlurVertical();
    }
}

technique Horizontal
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL BlurHorizontal();
    }
}



//Texture2D SpriteTexture;

//sampler2D SpriteTextureSampler = sampler_state
//{
//	Texture = <SpriteTexture>;
//};

//struct VertexShaderOutput
//{
//	float4 Position : SV_POSITION; 
//	float4 Color : COLOR0;
//	float2 TextureCoordinates : TEXCOORD0;
//};

//float4 MainPS(VertexShaderOutput input) : COLOR
//{
//	return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
//}

//technique SpriteDrawing
//{
//	pass P0
//	{
//		PixelShader = compile PS_SHADERMODEL MainPS();
//	}
//};