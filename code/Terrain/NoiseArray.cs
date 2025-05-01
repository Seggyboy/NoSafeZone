using Sandbox;
using Sandbox.Utility;
using System;

public sealed class NoiseArray : Component
{
	public static float[,] GenNoiseArray(int mapwidth, int mapheight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
	{
		float[,] noiseMap = new float[mapwidth, mapheight];

		System.Random prng = new System.Random( seed );
		Vector2[] octaveOffsets = new Vector2[octaves];
		for ( int i = 0; i< octaves; i++ )
		{
			float offsetX = prng.Next( -100000, 100000 ) + offset.x;
			float offsetY = prng.Next( -100000, 100000 ) + offset.y;
			octaveOffsets[i] = new Vector2( offsetX, offsetY );
		}

		if (scale <= 0 ) scale = 0.0001f;

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapwidth / 2f;
		float halfHeight = mapheight / 2f;

		for ( int y = 0; y < mapheight; y++ ){
			for ( int x = 0; x < mapwidth; x++ ){
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for ( int i = 0; i < octaves; i++ ){
					float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;
					float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y;

					float perlinValue = Noise.Perlin( sampleX, sampleY ) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if ( noiseHeight > maxNoiseHeight )
				{
					maxNoiseHeight = noiseHeight;
				}
				else if ( noiseHeight < minNoiseHeight )
				{
					minNoiseHeight = noiseHeight;
				}
				noiseMap[x, y] = noiseHeight;
				


			}
			

		}
		Log.Info( "Max NoiseHeight: " + maxNoiseHeight + " Min NoiseHeight: " + minNoiseHeight );
		
		for ( int y = 0; y < mapheight; y++ )
		{ 
			for ( int x = 0; x < mapwidth; x++ )
			{
				noiseMap[x, y] = InverseLerp( minNoiseHeight, maxNoiseHeight, noiseMap[x, y] );
				noiseMap[x,y] = MathF.Pow( noiseMap[x, y], 1.6f );
			}
		}
		return noiseMap;
	}

	public static float InverseLerp( float a, float b, float value )
	{
		return (value - a) / (b - a);
	}



}
