uniform extern texture ScreenTexture;

sampler ScreenS = sampler_state
{
	Texture = <ScreenTexture>;
};

float4 GammaShader(float2 texCoord : TEXCOORD0) : COLOR
{
	float4 color = tex2D(ScreenS, texCoord);
	float gamma = 1/1;
	color = pow(color, gamma);
	/*color.r = pow(color.r, gamma);
	color.g = pow(color.g, gamma);
	color.b = pow(color.b, gamma);
	color.a = pow(color.a, gamma);*/
	return color;
}
technique
{
	pass P0
	{
		PixelShader = compile ps_2_0 GammaShader();
	}
}
