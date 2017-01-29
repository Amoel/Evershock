sampler TextureSampler : register(s0);

texture bloom;
sampler bloomSampler = sampler_state
{
	Texture = <bloom>;
};

float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{    
	 float4 Color = tex2D(TextureSampler, coords); 
	 float4 BloomColor = tex2D(bloomSampler, coords);

	 return (Color + BloomColor * 0.7f);
}   

technique Technique1   
{   
	 pass Pass1   
	 {   
		 PixelShader = compile ps_4_0 PixelShaderFunction();
	 }   
}