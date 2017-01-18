sampler TextureSampler : register(s0);   
float BloomThreshold = 0.15f;   

float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{    
     float4 c = tex2D(TextureSampler, coords);   
  
     return saturate((c - BloomThreshold) / (1 - BloomThreshold));   
}   

technique Technique1   
{   
     pass Pass1   
     {   
         PixelShader = compile ps_4_0 PixelShaderFunction();
     }   
}