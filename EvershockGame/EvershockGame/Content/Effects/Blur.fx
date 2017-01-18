sampler TextureSampler : register(s0);
const float blurSizeHorizontal = 1.0f/960.0f;
const float blurSizeVertical = 1.0f/540.0f;

int dist = 100;

float4 HorizontalBlurFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float4 sum = float4(0.0, 0.0, 0.0, 0.0);
	float4 col = tex2D(TextureSampler, coords);

	for (int i = 0; i < dist; i++)
	{
		sum += tex2D(TextureSampler, float2(coords.x + (i - (float)dist / 2.0f) * blurSizeHorizontal, coords.y));
	}
	sum /= dist;
	return sum;
}

float4 VerticalBlurFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float4 sum = float4(0.0, 0.0, 0.0, 0.0);
	float4 col = tex2D(TextureSampler, coords);

	for (int i = 0; i < dist; i++)
	{
		sum += tex2D(TextureSampler, float2(coords.x, coords.y + (i - (float)dist / 2.0f) * blurSizeVertical));
	}
	sum /= dist;
	return sum;
}

technique Technique1
{
	pass HorizontalBlur
    {
        PixelShader = compile ps_4_0 HorizontalBlurFunction();
    }
}

technique Technique2
{
	pass VerticalBlur
    {
        PixelShader = compile ps_4_0 VerticalBlurFunction();
    }
}
