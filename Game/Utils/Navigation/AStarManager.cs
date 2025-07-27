using System;
using Sandbox;


public sealed class AStarManager : Component
{
	[Property] private Board board;
	[Property] private Tile[,] tiles;
	[Property] private AStarPathFinder pathfinder;
	public static AStarManager Instance { get; private set; }

	protected override void OnAwake()
	{
		Instance = this;
	}

	// optional: unregister
	public void DeInitialize()
	{
		if ( Instance == this )
			Instance = null;
		Log.Info( "Deinitializing AStarManager..." );
	}

	protected override void OnDisabled()
	{
		DeInitialize();
	}

	public void Initialize()
	{
		Log.Info( "Initializing AStarManager..." );	
		board = Scene.GetAllComponents<Board>().FirstOrDefault();
		tiles = board.World[0];
		pathfinder = new AStarPathFinder( board );
	}

	public List<Tile> RequestPath( Tile start, Tile end, bool UseReserveSystem )
	{
		if ( pathfinder == null )
		{
			Log.Error( "AStarPathFinder is not initialized." );
			return null;
		}
		return pathfinder.FindPath( start, end, UseReserveSystem );
	}

	public List<Tile> RequestTilesInRange( Tile start, int range )
	{
		if ( pathfinder == null )
		{
			Log.Error( "AStarPathFinder is not initialized." );
			return null;
		}
		return pathfinder.GetTilesInRange( start, range );
	}

	public float RequestDistance( Tile start, Tile end)
	{
		if ( start == null || end == null )
			return float.MaxValue;

		int dx = start.X - end.X;
		int dy = start.Y - end.Y;

		return MathF.Sqrt( dx * dx + dy * dy );
	}

	public Tile GetAdjacentFreeTile( Tile target, Tile currentTile )
	{
		for ( int radius = 1; radius <= 5; radius++ )
		{
			var neighbors = pathfinder.GetTilesInRange( target, radius );
			Tile bestTile = null;
			float bestDist = float.MaxValue;

			foreach ( var tile in neighbors )
			{
				if ( tile == null || tile == target ) continue;
				if ( tile.HasFlag( TileFlags.Occupied ) ) continue;
				
				tile.GetComponent<ModelRenderer>().Tint = ( Color.Green ); // Optional: visualize the tile
				float dist = Tile.Distance( currentTile, tile );
				if ( dist < bestDist )
				{
					bestDist = dist;
					bestTile = tile;
				}
			}

			if ( bestTile != null )
				return bestTile; // As soon as we find a valid one, return it
		}

		return null; // No valid tile found
	}


}



