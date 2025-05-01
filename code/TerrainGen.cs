using System;
using Sandbox;



public sealed class TerrainGen : Component
{
	
	[Property] public float NoiseScale;
	[Property] public int Octaves;
	[Property] public float Lacunarity;
	[Property] public float Persistance;
	[Property] public int Seed;
	[Property] public Vector2 Offset;
	
	[Property] Terrain terrain;
	[Property] public List<Region> regions = new List<Region>();

	float[,] NoiseMap;
	int Resolution;




	[Button]
	public void GenerateMap()
	{	
		AssignHeight();
		CreateSplatmap();
		terrain.SyncGPUTexture();	
	}

	public void AssignHeight()
	{
		Resolution = terrain.Storage.Resolution;
		NoiseMap = NoiseArray.GenNoiseArray( Resolution, Resolution, Seed, NoiseScale, Octaves, Persistance, Lacunarity, Offset );
		var heightmap = terrain.Storage.HeightMap;
		for ( int y = 0; y < NoiseMap.GetLength( 1 ); y++ )
		{
			for ( int x = 0; x < NoiseMap.GetLength( 0 ); x++ )
			{

				int index = y * NoiseMap.GetLength( 1 ) + x;
				heightmap[index] = (ushort)(NoiseMap[x, y] * terrain.TerrainHeight);

			}
		}
	}

	

	public Color32[] GenerateRegionMaterials( float[,] noiseMap )
	{
		Color32[] colors = new Color32[noiseMap.GetLength( 0 ) * noiseMap.GetLength( 1 )];
		TerrainMaterial[] materials = new TerrainMaterial[noiseMap.GetLength( 0 ) * noiseMap.GetLength( 1 )];

		for ( int y = 0; y < noiseMap.GetLength( 1 ); y++ )
		{
			for ( int x = 0; x < noiseMap.GetLength( 0 ); x++ )
			{
				
				


				float height = noiseMap[x, y];
				float slope = GetSlope(noiseMap, x, y, noiseMap.GetLength( 0 ), noiseMap.GetLength( 1 ) );
				
			

				
				

				// Control strength of each layer based on height

				float grassStrength = Saturate( 1.0f - MathF.Abs( height - 0.2f ) * 10.0f );
				float dirtStrength = Saturate( 1.0f - MathF.Abs( height - 0.5f ) * 10.0f );
				float rockStrength = Saturate( 1.0f - MathF.Abs( height - 0.75f ) * 10.0f );
				float snowStrength = Saturate( 1.0f - MathF.Abs( height - 0.9f ) * 10.0f );

				rockStrength = Math.Clamp( slope * 40f, 0f, 1f );

				// Steep slopes should *reduce* grass and dirt and snow a bit
				grassStrength *= (1f - rockStrength);
				dirtStrength *= (1f - rockStrength);
				snowStrength *= (1f - rockStrength);

				// Normalize them so they add up to 1
				float total = grassStrength + dirtStrength + rockStrength + snowStrength;
				if ( total <= 0.0001f )
				{
					grassStrength = 1f; // fallback
					dirtStrength = rockStrength = snowStrength = 0f;
					total = 1f;
				}

				grassStrength /= total;
					dirtStrength /= total;
					rockStrength /= total;
					snowStrength /= total;

					// Set splatmap pixel
					colors[y* noiseMap.GetLength( 0 ) + x] = new Color32(
						(byte)(grassStrength * 255),
						(byte)(dirtStrength * 255),
						(byte)(rockStrength * 255),
						(byte)(snowStrength * 255)
					);
					
				
			}
		}
	
		return colors;

	}

	// Assume you have a heightmap[x,y] with heights

	float GetSlope( float[,] heightmap, int x, int y, int width, int height )
	{
		float heightL = (x > 0) ? heightmap[x - 1, y] : heightmap[x, y];
		float heightR = (x < width - 1) ? heightmap[x + 1, y] : heightmap[x, y];
		float heightD = (y > 0) ? heightmap[x, y - 1] : heightmap[x, y];
		float heightU = (y < height - 1) ? heightmap[x, y + 1] : heightmap[x, y];

		float dX = heightR - heightL;
		float dY = heightU - heightD;

		// Slope magnitude (steepness)
		return MathF.Sqrt( dX * dX + dY * dY );
	}


	public static float Saturate( float value )
	{
		return MathF.Min( MathF.Max( value, 0f ), 1f );
	}


	public void CreateSplatmap()
	{
		

		
		List<TerrainMaterial> tmats = new List<TerrainMaterial>();
		var materials = GenerateRegionMaterials( NoiseMap );
		var heightmap = terrain.Storage.HeightMap;

		for ( int i = 0; i < regions.Count; i++)
		{
			tmats.Add( regions[i].tmat );
		}
		terrain.Storage.Materials = tmats;
		var splatmap = GenerateRegionMaterials( NoiseMap ); ;
		var pixelData = ColorsToBytes( splatmap );
		SaveToFile( pixelData, Resolution, Resolution, "splatmap" );
		terrain.Storage.ControlMap = splatmap;
		
		
		



	}

	
	
	public byte[] ColorsToBytes( Color32[]colors)
	{
		var width = colors.Length;
		byte[] pixelData = new byte[width * 4];

		
			for ( int x = 0; x < width; x++ )
			{


				int i = x * 4;
				pixelData[i + 0] = (byte)(colors[x].r ); // Red
				pixelData[i + 1] = (byte)(colors[x].g ); // Green
				pixelData[i + 2] = (byte)(colors[x].b ); // Blue
				pixelData[i + 3] = (byte)(255); // Alpha
				
				


			}
			return pixelData;

	}

	public void SaveToFile( byte[] pixelData, int width, int height, string filename )
	{
		var texture = Texture.Create( width, height, ImageFormat.RGBA8888 );
		texture.WithData( pixelData );
		var tex = texture.Finish();
		using var stream = FileSystem.Data.OpenWrite( filename + ".png", System.IO.FileMode.OpenOrCreate );
		stream.Write( tex.GetBitmap( 0 ).ToPng() );
		if ( FileSystem.Data.FileExists( filename+".png") )
		{
			Log.Info( "File created successfully!" );
		}
		else
		{
			Log.Error( "Failed to create file." );
		}
		stream.Close();
	}

	public struct Region
	{
		public string Name;
		public float minHeight;
		public float maxHeight;
		public Color Color;
		public TerrainMaterial tmat;
	}



}




