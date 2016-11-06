sampler textureSampler : register(s0);
		
texture lightMask;
sampler lightSampler = sampler_state 
{ 
	Texture = <lightMask>;
};
	
float4 PixelShaderLight(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords: TEXCOORD0) : COLOR0
{  
	float4 color = tex2D(textureSampler, coords);
	float4 lightColor = tex2D(lightSampler, coords);  

	float4 newColor = color * float4(max(lightColor.r, 0.1f), max(lightColor.g, 0.1f), max(lightColor.b, 0.1f), max(lightColor.a, 0.1f));
	//float brightness = (newColor.r + newColor.g + newColor.b) / 3.0f;
	//newColor.rgb = ((newColor.rgb - 0.5f) * max((3.0f - brightness), 0)) + 0.5f;
	return newColor;
}  

		
technique Technique1  
{  
	pass Pass1  
	{  
		PixelShader = compile ps_4_0 PixelShaderLight(); //ps_4_0_level_9_1
	}  
}  