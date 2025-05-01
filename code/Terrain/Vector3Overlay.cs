using Sandbox;
using System;

public sealed class Vector3Overlay : Component
{
	[Property] Terrain terrain;
	

	

	public List<Vector3> ValidVectors()
	{
		List<Vector3> validPoints = new List<Vector3>();
		var heightmap = terrain.Storage.HeightMap;
		var resolution = terrain.Storage.Resolution;
		float[,] heightmap2D = ThickenArray(resolution, heightmap );
		

		for ( int y = 0; y < heightmap2D.GetLength( 0 ); y++ )
		{
			for ( int x = 0; x < heightmap2D.GetLength( 0 ); x++ )
			{
				var worldX= (x / (float)terrain.Storage.Resolution ) * terrain.TerrainSize;
				var worldY = (y / (float)terrain.Storage.Resolution) * terrain.TerrainSize;
				var WorldZ = heightmap2D[x, y];
				validPoints.Add(new Vector3(worldX, worldY, WorldZ ) );

			}
		}
		return validPoints;
	}

	public float[,] ThickenArray(int resolution, ushort[] heightmap1D)
	{
		float[,] heightmap2D = new float[resolution, resolution];

		for ( int i = 0; i < heightmap1D.Length; i++ )
		{
			int x = i % resolution;
			int z = i / resolution;

			heightmap2D[x, z] = (heightmap1D[i] / 65535f) * terrain.TerrainHeight + 20;
		}

		return heightmap2D;
	}

	[Button( "Display Valid Vectors" )]

	public void DisplayValidVectors()
	{
		var thickVectors = ValidVectors();
		
		
	}



	
}
