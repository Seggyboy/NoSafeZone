using System;
using System.Diagnostics;
using Sandbox;
using Sandbox.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Astar;

public sealed class Astar : Component
{
	[Property] Terrain terrain;
	[Property] Vector3Overlay vector3dOverlay;
	public float spacing;
	[Property]public float targetx;
	[Property]public float targety;
	[Property] public float SlopePenalty;
	[Property] GameObject target;
	[Property] GameObject start;
	float[,] heightmap;
	public struct Node
	{
		public Vector3 position;



		public override bool Equals( object obj )
		{
			return obj is Node other && position == other.position;
		}

		public override int GetHashCode()
		{
			return position.GetHashCode();
		}

	}
	[Button( "Test Pathfinding" )]

	public void TestPathFinding()
	{
		Log.Info(FindPath(start.WorldPosition, target.WorldPosition ));
	}

	public List<Vector3> FindPath(Vector3 start, Vector3 end)
	{
		heightmap = ThickenArray( terrain.Storage.Resolution, terrain.Storage.HeightMap );
		float spacing = terrain.TerrainSize / (terrain.Storage.Resolution);
		Node startNode = new Node();
		startNode.position = start;
		Node endNode = new Node();
		endNode.position = end;
		var validStart = SnappedPosition(FindValidPoint( startNode ).position);
		var validEnd =SnappedPosition( FindValidPoint( endNode ).position);
		startNode.position = validStart;
		endNode.position = validEnd;



		PriorityQueue<Node, float> frontier = new PriorityQueue<Node, float>();
		HashSet<Node> closedSet = new HashSet<Node>();	
		Dictionary<Node, float> gScore = new Dictionary<Node, float>();
		Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();


		gScore[startNode] = 0f;
		frontier.Enqueue( startNode, 0f );
		//Log.Info( frontier.Count );

		while (frontier.Count > 0)
		{
			Node current = frontier.Dequeue();
			closedSet.Add( current );
			//Log.Info( "Frontier node has stuff in it." );
			if ( (current.position - endNode.position).Length < spacing * 3 )
			{
				
				return ReconstructPath( cameFrom, current );
				
			}
			var neighbors = GetNeighbors( current );
			foreach (Node neighbor in neighbors)
			{
				
				
				
				if ( closedSet.Contains( neighbor ) )
					continue;
				float tentativeGScore = gScore.GetValueOrDefault( current, float.MaxValue ) + GetCost( current, neighbor );
				//Log.Info( tentativeGScore );
				//Log.Info( gScore.GetValueOrDefault( neighbor, float.MaxValue ) );
				if ( tentativeGScore < gScore.GetValueOrDefault( neighbor, float.MaxValue ) )
				{
					cameFrom[neighbor] = current;
					gScore[neighbor] = tentativeGScore;
					float fScore = tentativeGScore + GetCost( neighbor, endNode );
					frontier.Enqueue( neighbor, fScore );
				}
			}
		}
		Log.Info( "Failed to create path!" );
		return null;



	}

	public List<Vector3> ReconstructPath( Dictionary<Node, Node> cameFrom, Node current )
	{
		List<Node> totalPath = new List<Node>();
		totalPath.Add( current );
		while ( cameFrom.ContainsKey( current ) )
		{
			current = cameFrom[current];
			totalPath.Add( current );
		}
		totalPath.Reverse();
		for ( int i = 0; i < totalPath.Count; i++ )
		{
			totalPath[i] = new Node() { position = SnappedPosition( totalPath[i].position )};
		
		}
		List<Vector3> path = new List<Vector3>();
		foreach ( Node node in totalPath )
		{
			path.Add( node.position );
		}
		return path;

	}

	public List<Node> GetNeighbors(Node node)
	{
		
	
		float spacing = terrain.TerrainSize / (terrain.Storage.Resolution);
		List<Node> neighbors = new List<Node>();
		
		var VectorOffsets = GetDirectionOffsets( spacing );
		foreach ( Vector3 offset in VectorOffsets )
		{
			
			Vector3 newPos = node.position + offset;
			var arrayY = (int)(newPos.y / spacing);
			var arrayX = (int)(newPos.x / spacing);
			if ( arrayX < 0 || arrayY < 0 || arrayX >= terrain.Storage.Resolution || arrayY >= terrain.Storage.Resolution )
				continue;
			var finalPos = new Vector3( newPos.x, newPos.y, heightmap[arrayX, arrayY] );
		
			neighbors.Add( new Node() { position = finalPos } );
			
		}
			
		
		
		

		return neighbors;
	}


	public float[,] ThickenArray( int resolution, ushort[] heightmap1D )
	{
		float[,] heightmap2D = new float[resolution, resolution];

		for ( int i = 0; i < heightmap1D.Length; i++ )
		{
			int x = i % resolution;
			int z = i / resolution;

			heightmap2D[x, z] = (heightmap1D[i] / 65535f) * terrain.TerrainHeight;
		}

		return heightmap2D;
	}

	public static List<Vector3> GetDirectionOffsets( float stepSize )
	{
		
		return new List<Vector3>
	{
		new Vector3(  stepSize,     0, 0), // East
		new Vector3(  stepSize,  stepSize, 0), // Northeast
		new Vector3(     0,    stepSize, 0), // North
		new Vector3( -stepSize,  stepSize, 0), // Northwest
		new Vector3( -stepSize,     0, 0), // West
		new Vector3( -stepSize, -stepSize, 0), // Southwest
		new Vector3(     0,   -stepSize, 0), // South
		new Vector3(  stepSize, -stepSize, 0), // Southeast
	};
	}

	public float GetCost(Node nodeOne, Node nodeTwo)
	{
		var slope = GetSlope( nodeOne, nodeTwo );
		var distance = Vector2.DistanceBetween( nodeOne.position, nodeTwo.position );
		float cost = distance + (slope * SlopePenalty);
		return cost;
	}

	public float GetSlope( Node nodeOne, Node nodeTwo )
	{
		float deltaz = MathF.Abs( nodeOne.position.z - nodeTwo.position.z );
		float distance = Vector2.DistanceBetween( nodeOne.position, nodeTwo.position );
		float slope = deltaz / (distance + 0.01f); // prevent div0
		return slope;
	}

	

	[Button( "Test Nodes" )]
	public void TestNodes()
	{
		
		
		Node startNode = new Node();
		startNode.position = new Vector3(700, 800, 9000 );
		var validNode = FindValidPoint( startNode );
		//Log.Info(validNode.position );
		Vector3 vec = validNode.position;

		GameObject go = new GameObject();
		go.AddComponent<ModelRenderer>();
		go.GetComponent<ModelRenderer>().Model = Model.Load( "models/dev/box.vmdl_c" );
		go.WorldPosition = vec;
		go.Name = "Initial Valid Node";

		

		





		List<Node> neighbors = GetNeighbors( validNode );


		



	}

	public Node FindValidPoint(Node node)
	{
		node.position = new Vector3( node.position.x, node.position.y, GetTerrainHeight(new Vector2 (node.position.x, node.position.y) ) );

		return node;

	}

	


	float SampleRawHeight( int x, int y )
	{
		int resolution = terrain.Storage.Resolution;
		if ( x < 0 || y < 0 || x >= resolution || y >= resolution ) return 0f;

		int index = y * resolution + x;
		ushort raw = terrain.Storage.HeightMap[index];
		return (raw / 65535f) * terrain.Storage.TerrainHeight;
	}


	public float GetTerrainHeight( Vector2 worldPos )
	{
		int resolution = terrain.Storage.Resolution;
		float terrainSize = terrain.TerrainSize;

		float fx = (worldPos.x / terrainSize) * (resolution - 1);
		float fy = (worldPos.y / terrainSize) * (resolution - 1);

		int x0 = (int)MathF.Floor( fx );
		int x1 = Math.Min( x0 + 1, resolution - 1 );
		int y0 = (int)MathF.Floor( fy );
		int y1 = Math.Min( y0 + 1, resolution - 1 );

		float h00 = SampleRawHeight( x0, y0 );
		float h10 = SampleRawHeight( x1, y0 );
		float h01 = SampleRawHeight( x0, y1 );
		float h11 = SampleRawHeight( x1, y1 );

		float tx = fx - x0;
		float ty = fy - y0;

		float h0 = MathX.Lerp( h00, h10, tx );
		float h1 = MathX.Lerp( h01, h11, tx );

		return MathX.Lerp( h0, h1, ty );
	}

	Vector3 SnappedPosition( Vector3 position )
	{
		var trace = Scene.Trace.Ray ( position + Vector3.Up * 1000000f, position + Vector3.Down * 10000000f )
			.WithTag( "terrain" )  // Make sure your terrain has this tag
			.WithCollisionRules( "terrain" )
			.Run();

		if ( trace.Hit )
		{
			return trace.EndPosition;
		}
		else
		{
			// fallback if terrain wasn't hit
			return position;
		}


	}


}


