sampler TextureSampler : register(s0);   
float BloomThreshold = 0.4f;   

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0   
{    
     float4 c = tex2D(TextureSampler, texCoord);   
  
     return saturate((c - BloomThreshold) / (1 - BloomThreshold));   
}   

technique BloomExtract   
{   
     pass Pass1   
     {   
         PixelShader = compile ps_4_0 PixelShaderFunction();
     }   
}