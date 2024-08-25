#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif



Texture2D SpriteTexture;

float strength; // passed in from C#
float2 textureSize; // passed in from C#

float threshold = 0.25; // 0.25 is pretty cool

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

//float3 HSVtoRGB(float3 hsv)
//{
//    float3 rgb;

//    float c = hsv.y * hsv.z;
//    float x = c * (1.0f - abs(fmod(hsv.x / 60.0f, 2.0f) - 1.0f));
//    float m = hsv.z - c;

//    if (hsv.x >= 0.0f && hsv.x < 60.0f)
//        rgb = float3(c, x, 0.0f);
//    else if (hsv.x >= 60.0f && hsv.x < 120.0f)
//        rgb = float3(x, c, 0.0f);
//    else if (hsv.x >= 120.0f && hsv.x < 180.0f)
//        rgb = float3(0.0f, c, x);
//    else if (hsv.x >= 180.0f && hsv.x < 240.0f)
//        rgb = float3(0.0f, x, c);
//    else if (hsv.x >= 240.0f && hsv.x < 300.0f)
//        rgb = float3(x, 0.0f, c);
//    else
//        rgb = float3(c, 0.0f, x);

//    rgb += m;

//    return rgb;
//}

//float3 RGBtoHSV(float3 rgb)
//{
//    float3 hsv;

//    float minChannel = min(min(rgb.r, rgb.g), rgb.b);
//    float maxChannel = max(max(rgb.r, rgb.g), rgb.b);
//    float delta = maxChannel - minChannel;

//    // Calculate hue (in degrees)
//    if (delta > 0.0f)
//    {
//        if (maxChannel == rgb.r)
//            hsv.x = 60.0f * (rgb.g - rgb.b) / delta;
//        else if (maxChannel == rgb.g)
//            hsv.x = 60.0f * ((rgb.b - rgb.r) / delta + 2.0f);
//        else
//            hsv.x = 60.0f * ((rgb.r - rgb.g) / delta + 4.0f);

//        if (hsv.x < 0.0f)
//            hsv.x += 360.0f;
//    }
//    else
//    {
//        hsv.x = 0.0f;
//    }

//    // Calculate saturation
//    hsv.y = (maxChannel > 0.0f) ? delta / maxChannel : 0.0f;

//    // Value (brightness) is the maximum channel value
//    hsv.z = maxChannel;

//    return hsv;
//}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    float4 color = tex2D(SpriteTextureSampler, coords);
    
    //if (input.TextureCoordinates.y <= strength)
    //{
    //    color.r = 0.2;
    //}
    
    float2 size = textureSize;
    
    float2 offset = 1.0f / size; // this is how big a pixel is in UV
    
    
    int neighbors = 1;
    for (int i = -neighbors; i <= neighbors; i++)
    {
        for (int j = -neighbors; j <= neighbors; j++)
        {
            // sample with the given offset
            if (i == 0 && j == 0)
                continue;

            float2 neighborCoords = saturate(coords + float2(offset.x * i, offset.y * j));
            float4 neighborColor = tex2D(SpriteTextureSampler, neighborCoords);

            // calculate weight based on the distance to neighbor (closer = more weight)
            float weight = (sqrt(neighbors * neighbors * 2) - length(float2(i, j))) * strength / sqrt(neighbors * neighbors * 2);

            // add the neighbor's influence to the current pixel's color
            color.rgb += neighborColor.rgb * weight;
        }
    }

    return color * input.Color;

}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}

    //float4 tex = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    //// Calculate the grayscale value (average of RGB channels)
    ////float grayscale = dot(tex.rgb, float3(0.3333, 0.3333, 0.3333));

    ////// Check if the grayscale value is above the threshold
    //float4 result = (tex > threshold) ? float4(1, 1, 1, 1) : float4(0, 0, 0, 1);

    //return result * input.Color;