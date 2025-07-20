using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

public class AStarPathFinder
{
	private Tile[,] tiles;
	private Board board;

	public AStarPathFinder( Board board )
	{
		this.board = board;
		this.tiles = board.World[0]; // Adjust if you have multiple layers
	}

	public List<Tile> FindPath( Tile start, Tile goal, bool UseReserve )
	{
		var openSet = new PriorityQueue<Tile>();
		var cameFrom = new Dictionary<Tile, Tile>();

		var gScore = new Dictionary<Tile, float>();
		var fScore = new Dictionary<Tile, float>();

		openSet.Enqueue( start, 0 );
		gScore[start] = 0;
		fScore[start] = Heuristic( start, goal );

		Tile closestTile = start;
		float closestHeuristic = Heuristic( start, goal );

		while ( openSet.Count > 0 )
		{
			var current = openSet.Dequeue();

			// If we reached the goal, return the full path
			if ( current == goal )
				return ReconstructPath( cameFrom, current );

			foreach ( var neighbor in GetNeighbors( current ) )
			{
				if ( neighbor == null || !IsWalkable( neighbor, UseReserve ) ) continue;

				float tentativeG = gScore[current] + Distance( current, neighbor );

				if ( !gScore.ContainsKey( neighbor ) || tentativeG < gScore[neighbor] )
				{
					cameFrom[neighbor] = current;
					gScore[neighbor] = tentativeG;
					fScore[neighbor] = tentativeG + Heuristic( neighbor, goal );

					// Track the closest tile seen so far
					float h = Heuristic( neighbor, goal );
					if ( h < closestHeuristic )
					{
						closestHeuristic = h;
						closestTile = neighbor;
					}

					if ( !openSet.Contains( neighbor ) )
						openSet.Enqueue( neighbor, fScore[neighbor] );
				}
			}
		}

		// If no full path found, return the path to the closest tile
		if ( closestTile != start )
			return ReconstructPath( cameFrom, closestTile );

		return null; // No progress made at all
	}

	private float Heuristic( Tile a, Tile b )
	{
		float dx = Math.Abs( a.X - b.X );
		float dy = Math.Abs( a.Y - b.Y );
		return (float) Math.Sqrt( dx * dx + dy * dy ); // Euclidean
	}

	private float Distance( Tile a, Tile b )
	{
		int dx = Math.Abs( a.X - b.X );
		int dy = Math.Abs( a.Y - b.Y );

		if ( dx + dy == 1 ) return 1f; // Cardinal move
		if ( dx == 1 && dy == 1 ) return 1.4142f; // Diagonal
		return 1000000000f; // Invalid move (should not happen)
	}

	private List<Tile> ReconstructPath( Dictionary<Tile, Tile> cameFrom, Tile current )
	{
		var path = new List<Tile> { current };
		while ( cameFrom.ContainsKey( current ) )
		{
			current = cameFrom[current];
			path.Insert( 0, current );
		}
		return path;
	}

	public IEnumerable<Tile> GetNeighbors( Tile tile )
	{
		int x = tile.X;
		int y = tile.Y;

		for ( int dx = -1; dx <= 1; dx++ )
			for ( int dy = -1; dy <= 1; dy++ )
			{
				if ( dx == 0 && dy == 0 ) continue;

				int nx = x + dx;
				int ny = y + dy;

				if ( board.IsValidTile( nx, ny ) )
					yield return tiles[nx, ny];
			}
	}

	private bool IsWalkable(Tile tile, bool UseReserveSystem = false)
	{
		if ( tile == null ) return false;
		if ( tile.HasFlag( TileFlags.Occupied ) || tile.HasFlag(TileFlags.FullCover) || tile.HasFlag(TileFlags.Wall)) return false;
		if ( UseReserveSystem && tile.ReservedBy != null ) return false;
		return true;
	}

	public List<Tile> GetTilesInRange( Tile start, int range )
	{
		var inRange = new List<Tile>();
		var visited = new HashSet<Tile>();
		var queue = new Queue<(Tile tile, int distance)>();

		queue.Enqueue( (start, 0) );
		visited.Add( start );

		while ( queue.Count > 0 )
		{
			var (current, distance) = queue.Dequeue();

			if ( distance > range )
				continue;

			inRange.Add( current );

			foreach ( var neighbor in GetNeighbors( current ) )
			{
				if ( neighbor == null || !IsWalkable( neighbor ) ) continue;

				if ( neighbor == null || visited.Contains( neighbor ) )
					continue;

				visited.Add( neighbor );
				queue.Enqueue( (neighbor, distance + 1) );
			}
		}

		return inRange;
	}
}



public class PriorityQueue<T>
{
	private readonly List<(T item, float priority)> elements = new();

	public int Count => elements.Count;

	public void Enqueue( T item, float priority )
	{
		elements.Add( (item, priority) );
	}

	public T Dequeue()
	{
		int bestIndex = 0;
		for ( int i = 1; i < elements.Count; i++ )
		{
			if ( elements[i].priority < elements[bestIndex].priority )
				bestIndex = i;
		}
		var bestItem = elements[bestIndex].item;
		elements.RemoveAt( bestIndex );
		return bestItem;
	}

	public bool Contains( T item )
	{
		return elements.Exists( e => EqualityComparer<T>.Default.Equals( e.item, item ) );
	}
}
