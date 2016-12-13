sampler TextureSampler : register(s0);
const float blurSizeHorizontal = 1.0f/480.0f;
const float blurSizeVertical = 1.0f/540.0f;

float4 HorizontalBlurFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float4 sum = float4(0.0, 0.0, 0.0, 0.0);
	float4 col = tex2D(TextureSampler, coords);

	int dist = clamp((int)(length(float2(0.5 - coords.x, 0.5 - coords.y)) * 150), 10, 100);
	for (int i = 0; i < dist; i++)
	{
		sum += tex2D(TextureSampler, float2(coords.x + (i - (float)dist / 2.0f) * blurSizeHorizontal, coords.y));
	}
	sum /= dist;

	if (col.a > 0.0f)
	{
		return sum;
	}
	else
	{
		return float4(0.0, 0.0, 0.0, 0.0);
	}
}

float4 VerticalBlurFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float4 sum = float4(0.0, 0.0, 0.0, 0.0);
	float4 col = tex2D(TextureSampler, coords);

	int dist = clamp((int)(length(float2(0.5 - coords.x, 0.5 - coords.y)) * 150), 10, 100);
	for (int i = 0; i < dist; i++)
	{
		sum += tex2D(TextureSampler, float2(coords.x, coords.y + (i - (float)dist / 2.0f) * blurSizeVertical));
	}
	sum /= dist;

	if (col.a > 0.0f)
	{
		return sum;
	}
	else
	{
		return float4(0.0, 0.0, 0.0, 1.0);
	}
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
