
#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Texture to sample
Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};
// Input structure from vertex shader
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

// Main pixel shader function
float4 MainPS(VertexShaderOutput input) : SV_Target
{
    // Sample the texture
    float2 coords = input.TextureCoordinates;
    float4 color = tex2D(SpriteTextureSampler, coords);

    // Apply any desired modifications (e.g., color tint, brightness, etc.)
    // Example: texColor.rgb *= float3(1.2, 1.0, 0.8); // Tint with a yellowish tone
    //float2 newpos = input.TextureCoordinates + float2(0, 2);
    //float4 neighborColor = tex2D(SpriteTextureSampler, newpos);
    
    //texColor.rgb *= float3(1.2, 1.0, neighborColor[2]);
    
    //int neighbors = 3;
    //for (int i = -neighbors; i <= neighbors; i++)
    //{
    //    for (int j = -neighbors; j <= neighbors; j++)
    //    {
    //        // sample with the given offset
    //        if (i == 0 && j == 0)
    //            continue;

    //        float2 neighborCoords = coords + float2(offset.x * i, offset.y * j);
    //        float4 neighborColor = tex2D(SpriteTextureSampler, neighborCoords);

    //        // calculate weight based on the distance to neighbor (closer = more weight)
    //        float weight = (sqrt(neighbors * neighbors * 2) - length(float2(i, j))) * strength / sqrt(neighbors * neighbors * 2);

    //        // add the neighbor's influence to the current pixel's color
    //        color.rgb += neighborColor.rgb * weight;
    //    }
    //}
    
    // Combine with vertex color
    //float4 finalColor = texColor * input.Color;

    return color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}