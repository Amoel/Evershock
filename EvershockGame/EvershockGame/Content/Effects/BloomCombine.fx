sampler TextureSampler : register(s0);
sampler Bloom : register(s1);

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0   
{    
     float4 Color = tex2D(TextureSampler, texCoord); 
     float4 BloomColor = tex2D(Bloom, texCoord); 

     return (Color + BloomColor);
}   

technique BloomExtract   
{   
     pass Pass1   
     {   
         PixelShader = compile ps_4_0 PixelShaderFunction();
     }   
}