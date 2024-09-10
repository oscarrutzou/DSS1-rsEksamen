#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float amount;
float sizeOfTeleport = 0.05f;

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
// The teleport effect. Works by checking the y axis of the tex coord and either showing it with an amount color or not
float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 col = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;

    if (input.TextureCoordinates.y <= amount)
    {
        col.rgba = 0;
    }
    else if (col.a > 0 && input.TextureCoordinates.y > amount && input.TextureCoordinates.y < amount + sizeOfTeleport)
    {
        col.rg = (col.r + col.g) / 2.0f;
        col.b = 1;
    }

    return col;
}

technique Teleport
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};


float4 RegularPS(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates);
	// Color here is the input color from spriteBatch.Draw(, ,, Color.White , , , );  white doesn't change anything.
    return color * input.Color;
}

technique Basic
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL RegularPS();
    }
};