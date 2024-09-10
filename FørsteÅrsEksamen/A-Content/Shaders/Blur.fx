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
float strength = 1.5f;
sampler2D TextureSampler = sampler_state
{
    texture = <Texture>;
    minfilter = point;
    magfilter = point;
    mipfilter = point;
};
// Problemet er at hvis den pos er 0, må den ikke blive fx 1
float4 BlurHorizontal(float4 position : SV_POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_Target
{
    float4 output = float4(0, 0, 0, 0);
    float2 coord;
    
    for (int i = 0; i < 7; i++)
    {
        coord = texCoord + float2(offsets[i], 0);
        if (coord.x >= 0.0 && coord.x <= 1.0)
        {
            float4 sample = float4(tex2D(TextureSampler, coord).rgb * weights[i] * strength, 0);
            output += sample;
        }
    }
    return output;
}

float4 BlurVertical(float4 position : SV_POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_Target
{
    float4 output = float4(0, 0, 0, 0);
    float2 coord;
    
    for (int i = 0; i < 7; i++)
    {
        coord = texCoord + float2(0, offsets[i]);
        if (coord.y >= 0.0 && coord.y <= 1.0)
            output += float4(tex2D(TextureSampler, coord).rgb * weights[i] * strength, 0);
    }
    return output;
}
//float4 BlurVertical(float4 position : SV_POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_Target
//{
//    float4 output = float4(0, 0, 0, 0);
//    float2 coord;

//    coord = texCoord + float2(0, offsets[0]);
//    if (coord.y >= 0.0 && coord.y <= 1.0)
//        output += float4(tex2D(TextureSampler, coord).rgb * weights[0], 0);

//    coord = texCoord + float2(0, offsets[1]);
//    if (coord.y >= 0.0 && coord.y <= 1.0)
//        output += float4(tex2D(TextureSampler, coord).rgb * weights[1], 0);

//    coord = texCoord + float2(0, offsets[2]);
//    if (coord.y >= 0.0 && coord.y <= 1.0)
//        output += float4(tex2D(TextureSampler, coord).rgb * weights[2], 0);

//    coord = texCoord + float2(0, offsets[3]);
//    if (coord.y >= 0.0 && coord.y <= 1.0)
//        output += float4(tex2D(TextureSampler, coord).rgb * weights[3], 0);

//    coord = texCoord + float2(0, offsets[4]);
//    if (coord.y >= 0.0 && coord.y <= 1.0)
//        output += float4(tex2D(TextureSampler, coord).rgb * weights[4], 0);

//    coord = texCoord + float2(0, offsets[5]);
//    if (coord.y >= 0.0 && coord.y <= 1.0)
//        output += float4(tex2D(TextureSampler, coord).rgb * weights[5], 0);

//    coord = texCoord + float2(0, offsets[6]);
//    if (coord.y >= 0.0 && coord.y <= 1.0)
//        output += float4(tex2D(TextureSampler, coord).rgb * weights[6], 0);

//    coord = texCoord + float2(0, offsets[7]);
//    if (coord.y >= 0.0 && coord.y <= 1.0)
//        output += float4(tex2D(TextureSampler, coord).rgb * weights[7], 0);

//    coord = texCoord + float2(0, offsets[8]);
//    if (coord.y >= 0.0 && coord.y <= 1.0)
//        output += float4(tex2D(TextureSampler, coord).rgb * weights[8], 0);

//    //coord = texCoord + float2(0, offsets[9]);
//    //if (coord.y >= 0.0 && coord.y <= 1.0)
//    //    output += float4(tex2D(TextureSampler, coord).rgb * weights[9], 0);

//    //coord = texCoord + float2(0, offsets[10]);
//    //if (coord.y >= 0.0 && coord.y <= 1.0)
//    //    output += float4(tex2D(TextureSampler, coord).rgb * weights[10], 0);

//    //coord = texCoord + float2(0, offsets[11]);
//    //if (coord.y >= 0.0 && coord.y <= 1.0)
//    //    output += float4(tex2D(TextureSampler, coord).rgb * weights[11], 0);

//    //coord = texCoord + float2(0, offsets[12]);
//    //if (coord.y >= 0.0 && coord.y <= 1.0)
//    //    output += float4(tex2D(TextureSampler, coord).rgb * weights[12], 0);

//    //coord = texCoord + float2(0, offsets[13]);
//    //if (coord.y >= 0.0 && coord.y <= 1.0)
//    //    output += float4(tex2D(TextureSampler, coord).rgb * weights[13], 0);

//    //coord = texCoord + float2(0, offsets[14]);
//    //if (coord.y >= 0.0 && coord.y <= 1.0)
//    //    output += float4(tex2D(TextureSampler, coord).rgb * weights[14], 0);

//    return output;
//}

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