uniform extern texture ScreenTexture;

sampler ScreenS = sampler_state
{
	Texture = <ScreenTexture>;
};

float4 GammaShader(float2 texCoord : TEXCOORD0) : COLOR
{
	// Get the color of a specific coordinate in the screen
	float4 color = tex2D(ScreenS, texCoord);

	// Calculate the gamma-corrected color
	float gamma = 1/1.5;
	color = pow(color, gamma);
	return color;
}
technique
{
	pass P0
	{
		PixelShader = compile ps_2_0 GammaShader();
	}
}
