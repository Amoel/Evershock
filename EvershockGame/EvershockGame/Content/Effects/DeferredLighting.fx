sampler textureSampler : register(s0);

bool isAmbientOcclusionEnabled;
		
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
	
float4 PixelShaderLight(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords: TEXCOORD0) : COLOR0
{  
	float4 color = tex2D(textureSampler, coords);
	float4 lightColor = tex2D(lightSampler, coords);  
	float4 shadowColor = tex2D(shadowSampler, coords);

	float4 newColor;
	if (lightColor.r > 0.0f)
	{
		newColor = color * float4(max(lightColor.b, 0.1f), max(lightColor.b, 0.1f), max(lightColor.b, 0.1f), max(lightColor.b, 0.1f));
	}
	else
	{
		newColor = color * float4(0.1f, 0.1f, 0.1f, 0.1f);
	}
	if (isAmbientOcclusionEnabled) newColor *= max(0.3f, shadowColor.a);
	return newColor;

	//float4 newColor = color * float4(max(lightColor.r, 0.1f), max(lightColor.g, 0.1f), max(lightColor.b, 0.1f), max(lightColor.a, 0.1f));
	/*if (isAmbientOcclusionEnabled) newColor *= max(0.3f, shadowColor.a);
	return newColor;*/
}  

		
technique Technique1  
{  
	pass Pass1  
	{  
		PixelShader = compile ps_4_0 PixelShaderLight(); //ps_4_0_level_9_1
	}  
}  