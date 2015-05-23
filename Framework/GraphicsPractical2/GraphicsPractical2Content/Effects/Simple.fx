//------------------------------------------- Defines -------------------------------------------

#define Pi 3.14159265

//------------------------------------- Top Level Variables -------------------------------------

// Top level variables can and have to be set at runtime

// Matrices for 3D perspective projection 
float4x4 View, Projection, World;

float4 LightingPosition;
float4 DiffuseColor;

float4 AmbientColor;
float AmbientIntensity;

float4 SpecularColor;
float SpecularIntensity;
float SpecularPower;

//---------------------------------- Input / Output structures ----------------------------------

// Each member of the struct has to be given a "semantic", to indicate what kind of data should go in
// here and how it should be treated. Read more about the POSITION0 and the many other semantics in 
// the MSDN library
struct VertexShaderInput
{
	float4 Position3D : POSITION0;
	float4 Normal : NORMAL0;
};

// The output of the vertex shader. After being passed through the interpolator/rasterizer it is also 
// the input of the pixel shader. 
// Note 1: The values that you pass into this struct in the vertex shader are not the same as what 
// you get as input for the pixel shader. A vertex shader has a single vertex as input, the pixel 
// shader has 3 vertices as input, and lets you determine the color of each pixel in the triangle 
// defined by these three vertices. Therefor, all the values in the struct that you get as input for 
// the pixel shaders have been linearly interpolated between there three vertices!
// Note 2: You cannot use the data with the POSITION0 semantic in the pixel shader.
struct VertexShaderOutput
{
	float4 Position2D : POSITION0;
	float4 Normal : TEXCOORD1;
	float4 Position3D : TEXCOORD2;
};

//------------------------------------------ Functions ------------------------------------------

// Implement the Coloring using normals assignment here
float4 NormalColor(float4 Normal)
{
	return float4((Normal.x + 1) / 2, (Normal.y + 1) / 2, (Normal.z + 1) / 2, 0);
}

// Implement the Procedural texturing assignment here
float4 ProceduralColor(float4 pos, float4 Normal)
{
	float4 color = Normal;
	if (sin(4*Pi * pos.x) > 0)
		color = -color;

	if (sin(4*Pi * pos.y) > 0)
		color = -color;

	return color;	
}

float4 LambertianShading(float4 Position, float4 Normal)
{
	return DiffuseColor * max(0, dot(normalize(Normal), normalize(LightingPosition - Position)));
}

float4 AmbientShading()
{
	return AmbientColor * AmbientIntensity;
}

float4 BlinnPhongShading(float4 Position, float4 Normal)
{  //WERKT NOG NIET
	float4 viewPosition = float4(0, 50, 100, 0);
	float4 viewDirection = normalize(viewPosition - Position);
	float4 lightDirection = normalize(LightingPosition - Position);

	float3 H = normalize(lightDirection + viewDirection);
	float NdotH = dot(Normal, H);
	float intensity = pow(saturate(NdotH), SpecularIntensity);

	float distance = length(LightingPosition - Position);
	return intensity * SpecularColor * SpecularPower / distance;
}

//---------------------------------------- Technique: Simple ----------------------------------------

VertexShaderOutput SimpleVertexShader(VertexShaderInput input)
{
	// Allocate an empty output struct
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Do the matrix multiplications for perspective projection and the world transform
	float4 worldPosition = mul(input.Position3D, World);
    float4 viewPosition  = mul(worldPosition, View);
	output.Position2D    = mul(viewPosition, Projection);
	output.Position3D = input.Position3D;
	
	float3x3 rotationAndScale = (float3x3)World;
	float3 newNormal = normalize(mul(input.Normal, rotationAndScale));
	output.Normal.xyz = newNormal;

	return output;
}

float4 SimplePixelShader(VertexShaderOutput input) : COLOR0
{
	float4 color = LambertianShading(input.Position3D, input.Normal) + AmbientShading();// +BlinnPhongShading(input.Position3D, input.Normal);

	return color;
}

technique Simple
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 SimpleVertexShader();
		PixelShader  = compile ps_2_0 SimplePixelShader();
	}
}