using Sandbox;
using Sandbox.Services;
using System;

public sealed class WorldSim : Component
{
	[Property] Terrain terrain;
	[Property] Astar astar;
	[Property] int desiredTownCount = 3; // Number of towns to generate
	List<Vector3> towns = new List<Vector3>();
	[Property]List<Spline> townPaths = new List<Spline>();


	[Button("Generate Towns")]

	public void GenerateTowns()
	{
		towns.Clear();
		townPaths.Clear();
		// Generate towns

		// Flatten terrain around each town
		foreach ( var town in PickTownSpots() )
		{
			towns.Add( town );
			GameObject go = new GameObject();
			go.AddComponent<ModelRenderer>();
			go.GetComponent<ModelRenderer>().Model = Model.Load( "models/dev/box.vmdl_c" );
			go.WorldPosition = town;
			go.Name = "Town Origin";
		}
		// Sync the terrain to apply changes
		terrain.SyncGPUTexture();
		ConnectTowns();

		

	}
	public List<Vector3> PickTownSpots()
	{

		List<Vector3> generatedTowns = new List<Vector3>();
		Random rand = new Random((int)Time.Now); 
		int attempts = 0;

		while ( generatedTowns.Count < desiredTownCount && attempts < 1000 )
		{
			float x = rand.Float( terrain.TerrainSize * 0.1f, terrain.TerrainSize * 0.9f );
			float y = rand.Float( terrain.TerrainSize * 0.1f, terrain.TerrainSize * 0.9f );
			float z = astar.GetTerrainHeight( new Vector2( x, y ) ); // reuse your existing method

			generatedTowns.Add( new Vector3( x, y, z ) );

			attempts++;
		}
		return generatedTowns;
	}

	public void AssignSplines( List<Vector3> path )
	{
		Spline spline = new Spline();
		for (int i = 0; i < path.Count; i+=10 )
		{
			var point = new Spline.Point();
			point.Position = path[i];
			spline.AddPoint( point );
		}
		townPaths.Add( spline );
	}

	void FlattenAroundArea( Vector3 area, float innerRadius, float outerRadius )
	{
		int resolution = terrain.Storage.Resolution;
		float spacing = terrain.TerrainSize / resolution;
		var heightmap = terrain.Storage.HeightMap;

		float targetHeight = area.z;

		for ( int y = 0; y < resolution; y++ )
		{
			for ( int x = 0; x < resolution; x++ )
			{
				float worldX = x * spacing;
				float worldY = y * spacing;

				float dist = Vector2.Distance( new Vector2( worldX, worldY ), new Vector2( area.x, area.y ) );

				if ( dist < outerRadius )
				{
					int index = y * resolution + x;

					if ( dist < innerRadius )
					{
						// Fully flatten within inner radius
						heightmap[index] = (ushort)((targetHeight / terrain.TerrainHeight) * 65535);
					}
					else
					{
						// Linearly blend from town height to existing terrain
						float t = (dist - innerRadius) / (outerRadius - innerRadius);
						float currentHeight = (heightmap[index] / 65535f) * terrain.TerrainHeight;
						float blendedHeight = MathX.Lerp( targetHeight, currentHeight, t );

						heightmap[index] = (ushort)((blendedHeight / terrain.TerrainHeight) * 65535);
					}
				}
			}
		}
		
	}


	/*void FlattenPath( List<Vector3> path, float roadWidth )
	{
		int resolution = terrain.Storage.Resolution;
		float spacing = terrain.TerrainSize / resolution;
		var heightmap = terrain.Storage.HeightMap;

		for ( int i = 1; i < path.Count - 1; i++ )
		{
			Vector3 current = path[i];
			Vector3 forward = (path[i + 1] - path[i - 1]).Normal;
			Vector3 right = Vector3.Cross( forward, Vector3.Up ).Normal;

			// World to array position
			int centerX = (int)(current.x / spacing);
			int centerY = (int)(current.y / spacing);

			int halfWidth = (int)(roadWidth / (2 * spacing));

			for ( int w = -halfWidth; w <= halfWidth; w++ )
			{
				Vector3 offset = right * w * spacing;
				Vector3 worldPos = current + offset;

				int x = (int)(worldPos.x / spacing);
				int y = (int)(worldPos.y / spacing);

				if ( x < 0 || y < 0 || x >= resolution || y >= resolution )
					continue;

				int index = y * resolution + x;
				float originalHeight = heightmap[index];

				Vector3 center = current;
				int cx = (int)(center.x / spacing);
				int cy = (int)(center.y / spacing);

				float centerHeight = heightmap[cy * resolution + cx];
				// Smooth falloff (cubic interpolation)
				float t = MathF.Abs( w ) / (float)halfWidth;
				t = t * t * (3 - 2 * t); // smoothstep falloff

				// Find average height of surrounding points if you want to preserve natural elevation
				float blended = MathX.Lerp( centerHeight, originalHeight, t );
				heightmap[index] = (ushort) blended;
			}
		}

		terrain.Storage.HeightMap = heightmap;

	}*/



	public struct Edge
	{
		public int A;
		public int B;
		public float distance;
		public Vector3 startPos;
		public Vector3 endPos;
		public Edge( int start, int end, float distance, Vector3 startPos, Vector3 endPos )
		{
			A = start;
			B = end;
			this.distance = distance;
			this.startPos = startPos;
			this.endPos = endPos;

		}
	}

	int[] parent;

	int Find( int x )
	{
		if ( parent[x] != x )
			parent[x] = Find( parent[x] );
		return parent[x];
	}

	void Union( int x, int y )
	{
		int rootX = Find( x );
		int rootY = Find( y );
		if ( rootX != rootY )
			parent[rootX] = rootY;
	}
	public void ConnectTowns()
	{
		//Get all the edges
		
		List<Edge> edges = new List<Edge>();
		
		for ( int i = 0; i < towns.Count; i++ )
		{
			for ( int j = i + 1; j < towns.Count; j++ )
			{
				
				float dist = Vector3.DistanceBetweenSquared( towns[i], towns[j] );
				edges.Add( new Edge( i, j, dist, towns[i], towns[j] ) );
			}
		}
		//Sort Edges by distance

		edges.Sort( ( e1, e2 ) => e1.distance.CompareTo( e2.distance ) );

		//Initialize MST
		parent = new int[towns.Count];
		for ( int i = 0; i < towns.Count; i++ ) parent[i] = i;

		List<Edge> mst = new();

		foreach ( Edge edge in edges )
		{
			if ( Find( edge.A ) != Find( edge.B ) )
			{
				mst.Add( edge );
				Union( edge.A, edge.B );
			}

			if ( mst.Count == towns.Count - 1 )
				break;
		}
		Log.Info( mst.Count + " MST HAS THIS MANY EDGES: " );
		foreach ( var edge in mst )
		{
			Log.Info(edge.startPos + " " + edge.endPos );	
			var path = astar.FindPath( edge.startPos, edge.endPos );
			if ( path == null )
				continue;
			AssignSplines( path );
			FlattenAroundArea( edge.startPos, 4000f, 18000 ); // Adjust radius as needed

		}



	}

	public float GetPathDistanceSquared(List<Vector3> path)
	{
		float total = 0f;
		for ( int i = 1; i < path.Count; i+=10 )
		{
			total += Vector3.DistanceBetweenSquared( path[i - 1], path[i] );
		}
		return total;
	}
}
