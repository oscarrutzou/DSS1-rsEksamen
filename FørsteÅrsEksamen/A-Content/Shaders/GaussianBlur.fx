#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float blurAmount;

sampler2D SpriteTextureSampler : register(s0)
{
    Texture = (Texture);
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    float blurScaledAmount;
    blurScaledAmount = blurAmount * 0.0006f;
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(blurScaledAmount, 0.00f));
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-blurScaledAmount, 0.00f));
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(2.0f * blurScaledAmount, 0.00f) * 0.5f);
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-2.0f * blurScaledAmount, 0.00f) * 0.5f);
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(3.0f * blurScaledAmount, 0.00f) * 0.25f);
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-3.0f * blurScaledAmount, 0.00f) * 0.25f);

    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.00f, blurScaledAmount));
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.00f, -blurScaledAmount));
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.00f, 2.0f * blurScaledAmount) * 0.5f);
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.00f, -2.0f * blurScaledAmount) * 0.5f);
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.00f, 3.0f * blurScaledAmount) * 0.25f);
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.00f, -3.0f * blurScaledAmount) * 0.25f);

    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(blurScaledAmount, blurScaledAmount) * 0.75f);
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-blurScaledAmount, blurScaledAmount) * 0.75f);
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-blurScaledAmount, -blurScaledAmount) * 0.75f);
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(blurScaledAmount, -blurScaledAmount) * 0.75f);
    
    // normal is 17
    color /= 17.0f; // Average the colors to make the colors apear
    
    return color * input.Color;
}


technique Blur
{
    pass P0
    {
        PixelShader = compile ps_4_0_level_9_1 MainPS();
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
        PixelShader = compile ps_4_0_level_9_1 RegularPS();
    }
};


//#if OPENGL
//#define SV_POSITION POSITION
//#define VS_SHADERMODEL vs_3_0
//#define PS_SHADERMODEL ps_3_0
//#else
//#define VS_SHADERMODEL vs_4_0_level_9_1
//#define PS_SHADERMODEL ps_4_0_level_9_1
//#endif

//float blurAmount;

//sampler2D SpriteTextureSampler : register(s0)
//{
//    Texture = (Texture);
//};

//struct VertexShaderOutput
//{
//    float4 Position : SV_POSITION;
//    float4 Color : COLOR0;
//    float2 TextureCoordinates : TEXCOORD0;
//};

//float4 MainPS(VertexShaderOutput input) : COLOR
//{
//    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates);
//    float blurScaledAmount;
//    blurScaledAmount = blurAmount * 0.0006f;
//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(blurScaledAmount, 0.00f));
//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-blurScaledAmount, 0.00f));
//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(2.0f * blurScaledAmount, 0.00f) * 0.5f);
//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-2.0f * blurScaledAmount, 0.00f) * 0.5f);
//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(3.0f * blurScaledAmount, 0.00f) * 0.25f);
//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-3.0f * blurScaledAmount, 0.00f) * 0.25f);

//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.00f, blurScaledAmount));
//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.00f, -blurScaledAmount));
//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.00f, 2.0f * blurScaledAmount) * 0.5f);
//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.00f, -2.0f * blurScaledAmount) * 0.5f);
//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.00f, 3.0f * blurScaledAmount) * 0.25f);
//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.00f, -3.0f * blurScaledAmount) * 0.25f);

//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(blurScaledAmount, blurScaledAmount) * 0.75f);
//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-blurScaledAmount, blurScaledAmount) * 0.75f);
//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-blurScaledAmount, -blurScaledAmount) * 0.75f);
//    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(blurScaledAmount, -blurScaledAmount) * 0.75f);

//    color /= 17.0f; // Average the colors to make the colors apear
    
//    return color * input.Color;
//}


//technique Blur
//{
//    pass P0
//    {
//        PixelShader = compile PS_SHADERMODEL MainPS();
//    }
//};

//// Calculate the grayscale value (average of RGB channels)
    //float grayscale = dot(color.rgb, float3(0.3333, 0.3333, 0.3333));

    ////// Check if the grayscale value is above the threshold
    //float4 result = (grayscale > threshold) ? float4(1, 1, 1, 1) : float4(0, 0, 0, 0);

    //return result * input.Color;
    // Apply any desired modifications (e.g., color tint, brightness, etc.)
    // Example: texColor.rgb *= float3(1.2, 1.0, 0.8); // Tint with a yellowish tone
