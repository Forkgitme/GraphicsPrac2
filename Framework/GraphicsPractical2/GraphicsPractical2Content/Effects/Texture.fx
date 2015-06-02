float4x4 World;
float4x4 View;
float4x4 Projection;

texture2D tex;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TextureUV : TEXCOORD0;

};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TextureUV : TEXCOORD0;

};

SamplerState TextureSampler
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	output.TextureUV = input.TextureUV;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return tex.Sample(TextureSampler, input.TextureUV);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
