sampler textureSampler : register(s0);

bool isAmbientOcclusionEnabled;
float verticalOffset = 0.0f;
float horizontalOffset = 0.0f;
float time = 10.0f;
		
texture lightMask;
sampler lightSampler = sampler_state 
{ 
	Texture = <lightMask>;
};

texture shadowMask;
sampler shadowSampler = sampler_state
{
	Texture = <shadowMask>;
};

float hash(float n)
{
	return frac(sin(n)*43758.5453);
}

float noise(float3 x)
{
	float3 p = floor(x);
	float3 f = frac(x);

	f = f*f*(3.0 - 2.0*f);
	float n = p.x + p.y*57.0 + 113.0*p.z;

	return lerp(lerp(lerp(hash(n + 0.0), hash(n + 1.0), f.x),
		lerp(hash(n + 57.0), hash(n + 58.0), f.x), f.y),
		lerp(lerp(hash(n + 113.0), hash(n + 114.0), f.x),
			lerp(hash(n + 170.0), hash(n + 171.0), f.x), f.y), f.z);
}
	
float4 PixelShaderLight(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords: TEXCOORD0) : COLOR0
{  
	float4 color = tex2D(textureSampler, coords);
	float4 lightColor = tex2D(lightSampler, coords);  
	float4 shadowColor = tex2D(shadowSampler, coords);

	float4 lightIntensity = float4(max(lightColor.r, 0.03f), max(lightColor.g, 0.03f), max(lightColor.b, 0.05f), max(lightColor.a, 0.05f));
	
	float4 newColor = color * lightIntensity;

	//float noiseValue = noise(float3(coords.x * 5.0f + horizontalOffset / 100.0f, coords.y * 5.0f + verticalOffset / 100.0f, time));
	//float fog = 1.0f - ((1.0f - lightIntensity) * noiseValue);
	
	//newColor *= fog;
		
	if (isAmbientOcclusionEnabled) newColor *= max(0.3f, shadowColor.a);
	return newColor;
}  

		
technique Technique1  
{  
	pass Pass1  
	{  
		PixelShader = compile ps_4_0 PixelShaderLight(); //ps_4_0_level_9_1
	}  
}  