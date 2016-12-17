sampler textureSampler : register(s0);
		
texture mask;
sampler maskSampler = sampler_state 
{ 
	Texture = <lightMask>;
};
	
float4 PixelShaderLight(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords: TEXCOORD0) : COLOR0
{  
	float4 color = tex2D(textureSampler, coords);
	float4 maskColor = tex2D(maskSampler, coords);

	if (maskColor.a > 0.0) return color;
	else return float4(0.0, 0.0, 0.0, 0.0);
}  

		
technique Technique1  
{  
	pass Pass1  
	{  
		PixelShader = compile ps_4_0 PixelShaderLight(); //ps_4_0_level_9_1
	}  
}  