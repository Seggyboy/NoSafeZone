using System;
using System.Collections.Generic;
using Sandbox;

public sealed class FogOfWar : Component
{
	[Property] private Board board;
	[Property] private BattleMaster battleMaster;

	[Property] private HashSet<Tile> previouslyVisible = new();

	[Property] private int activeZ = 0;

	private Action<int> zChangedHandler;
	protected override void OnEnabled()
	{
		
		board = Scene.GetAllComponents<Board>().FirstOrDefault();
		TacticalEvents.LevelLoadedEvent += InitializeVisibility;
		TacticalEvents.StartPlayerTurnEvent += SetVisibility;
		zChangedHandler = z =>
		{
			activeZ = z;
			SetVisibility();
			RenderLayers(); // force re-render on Z-layer switch
			
		};
		TacticalEvents.ChangedLayerZEvent += zChangedHandler;



	}

	protected override void OnDisabled()
	{

		board = null;
		TacticalEvents.LevelLoadedEvent -= InitializeVisibility;
		TacticalEvents.StartPlayerTurnEvent -= SetVisibility;
		TacticalEvents.ChangedLayerZEvent -= zChangedHandler;


	}

	public void RenderLayers()
	{
		//Log.Info( "Clearing layers..." );
		foreach ( var kvp in board.World )
		{
			int layer = kvp.Key;
			Tile[,] tileGrid = kvp.Value;
			int width = tileGrid.GetLength( 0 );
			int height = tileGrid.GetLength( 1 );
			for ( int x = 0; x < width; x++ )
			{
				for ( int y = 0; y < height; y++ )
				{
					var tile = tileGrid[x, y];
					if ( tile == null ) continue;
					
					ApplyTileRendering( tile );
					
				}
			}
		}
	}



	public void InitializeVisibility()
	{
		Log.Info( "Initializing visibility..." );
		foreach ( var kvp in board.World )
		{
			int layer = kvp.Key;
			Tile[,] tileGrid = kvp.Value;

			int width = tileGrid.GetLength( 0 );
			int height = tileGrid.GetLength( 1 );

			for ( int x = 0; x < width; x++ )
			{
				for ( int y = 0; y < height; y++ )
				{
					var tile = tileGrid[x, y];
					if ( tile == null ) continue;

					
					
						SetTileVisibility( tile, false, true );
					
				}
			}
		}
		
	}


	public void ClearVisibility()
	{

		foreach ( var tile in previouslyVisible )
		{
			SetTileVisibility( tile, false, false );
		}
		previouslyVisible.Clear();
	}

	public void ApplyTileRendering(Tile tile)
	{
		if ( tile == null ) return;
		var tileRenderer = tile.GetComponent<ModelRenderer>();
		var unitRenderer = tile.unit?.GetComponent<ModelRenderer>();
		var detailObjectRenderer = tile.DetailObject?.GetComponent<ModelRenderer>();
		var foliageObjectRenderer = tile.FoliageObject?.GetComponent<ModelRenderer>();
		var wallObjectRenderer = tile.WallObject?.GetComponent<ModelRenderer>();

		if ( tile.Z > activeZ)
		{
			if ( tileRenderer != null )
				tileRenderer.Tint = Color.Transparent;
			if ( unitRenderer != null )
				unitRenderer.Tint = Color.Transparent;
			if ( detailObjectRenderer != null )
				detailObjectRenderer.Tint = Color.Transparent;
			if ( foliageObjectRenderer != null )
				foliageObjectRenderer.Tint = Color.Transparent;
			if ( wallObjectRenderer != null )
				wallObjectRenderer.Tint = Color.Transparent;
			return; // Skip rendering for tiles above the active Z layer
		}
		else if ( !tile.Visible )
		{
			
			

			if ( tileRenderer != null )
				tileRenderer.Tint = tile.Explored ? Color.Gray : Color.Black;

			if ( unitRenderer != null )
				unitRenderer.Tint = Color.Transparent;

			if ( detailObjectRenderer != null )
				detailObjectRenderer.Tint = tile.Explored ? Color.Gray : Color.Black;
			if ( foliageObjectRenderer != null )
				foliageObjectRenderer.Tint = tile.Explored ? Color.Gray : Color.Transparent;
			if ( wallObjectRenderer != null )
				wallObjectRenderer.Tint = tile.Explored ? Color.Gray : Color.Black;
			
		

		}
		else if ( tile.Visible )
		{

			
			if ( tileRenderer != null )
				tileRenderer.Tint = Color.White;

			if ( tile.unit != null && unitRenderer != null )
				unitRenderer.Tint = Color.White;

			if ( detailObjectRenderer != null )
				detailObjectRenderer.Tint = Color.White;

			if ( foliageObjectRenderer != null )
				foliageObjectRenderer.Tint = Color.White;
			if ( wallObjectRenderer != null )
				wallObjectRenderer.Tint = Color.White;
			
		


		}
		
	}
	
	public void SetTileVisibility( Tile tile, bool visible, bool first )
	{

		

		if ( first )
		{

			tile.SetFlag( TileFlags.Visible, false );
			tile.SetFlag( TileFlags.Explored, false );
			ApplyTileRendering( tile );








		}
		else if ( !visible )
		{
			tile.SetFlag( TileFlags.Visible, false );
			ApplyTileRendering( tile );
			
			
		}
		else if ( visible )
		{
			tile.SetFlag( TileFlags.Visible, true );
			tile.SetFlag( TileFlags.Explored, true );
			ApplyTileRendering( tile );


		}




	}


	

	public void SetVisibility()
	{
		//Log.Info( "SetVisibility called" );
		ClearVisibility();

		var playerUnits = BattleMaster.Instance?.PlayerUnitsList;
		if ( playerUnits != null )
		{
			foreach ( var go in playerUnits )
			{
				var unit = go.GetComponent<Unit>();
				if ( unit?.CurrentTile == null ) continue;
				ComputeFOV( unit, unit.VisionRadius, unit.FOV );
				ApplyTileRendering(unit.CurrentTile);
			}
		}
		

	}

	private void ComputeFOV( Unit unit, int visionRadius, float fovDegrees )
	{
		if ( unit.CurrentTile == null ) return;

		Vector3Int start = new( unit.CurrentTile.X, unit.CurrentTile.Y, unit.CurrentTile.Z );
		Vector2 forward = new Vector2( unit.Forward.x, unit.Forward.y ).Normal;
		float halfFOV = fovDegrees / 2f;
		int rayCount = 220;

		for ( int i = 0; i < rayCount; i++ )
		{
			
			float angle = -halfFOV + (fovDegrees * i / (rayCount - 1));
			Vector2 dir2D = Rotate( forward, angle ).Normal;

			for ( int dz = -1; dz <= 3; dz++ ) // You can expand this to full vertical radius if needed
			{
				int targetZ = start.z + dz;
				if ( targetZ < 0 || targetZ >= board.World.Count ) continue;

				Vector3Int end = new(
					Math.Clamp( (int)MathF.Round( start.x + dir2D.x * visionRadius ), 0, board.Width - 1 ),
					Math.Clamp( (int)MathF.Round( start.y + dir2D.y * visionRadius ), 0, board.Length - 1 ),
					targetZ
				);

				CastRay3D( start, end );
			}
		}
	}



	/*private void CastRay( int x0, int y0, int x1, int y1, Unit unit )
	{
		foreach ( var tile in BresenhamLine( x0, y0, x1, y1 ) )
		{
			previouslyVisible.Add( tile );
			if ( !tile.HasFlag( TileFlags.Visible ) )
			{
				
	
				SetTileVisibility( tile, true, false );






			}

			if ( tile.BlocksVision )
				break;
		}
	}*/ //obsolete method for 2D

	private IEnumerable<Tile> Bresenham3D( int x0, int y0, int z0, int x1, int y1, int z1 )
	{
		int dx = Math.Abs( x1 - x0 ), sx = x0 < x1 ? 1 : -1;
		int dy = Math.Abs( y1 - y0 ), sy = y0 < y1 ? 1 : -1;
		int dz = Math.Abs( z1 - z0 ), sz = z0 < z1 ? 1 : -1;
		int dm = Math.Max( dx, Math.Max( dy, dz ) ); // Maximum difference
		int i = dm;

		int x = x0, y = y0, z = z0;
		int errX = dm / 2, errY = dm / 2, errZ = dm / 2;

		while ( i-- >= 0 )
		{
			var tile = board.GetTile( x, y, z );
			if ( tile is not null )
				yield return tile;

			errX -= dx;
			errY -= dy;
			errZ -= dz;

			if ( errX < 0 )
			{
				errX += dm;
				x += sx;
			}

			if ( errY < 0 )
			{
				errY += dm;
				y += sy;
			}

			if ( errZ < 0 )
			{
				errZ += dm;
				z += sz;
			}
		}
	}
	private void RevealNeighboringWalls( Tile tile )
	{
		int x = tile.X;
		int y = tile.Y;
		int z = tile.Z;

		// Check tile to the north (does it have a SouthWall facing into this one?)
		var north = board.GetTile( x, y - 1, z );
		if ( north != null && north.HasFlag( TileFlags.SouthWall ) )
		{
			var wallRenderer = north.WallObject?.GetComponent<ModelRenderer>();
			if ( wallRenderer != null )
				wallRenderer.Tint = Color.White;
		}

		// South (facing into this tile)
		var south = board.GetTile( x, y + 1, z );
		if ( south != null && south.HasFlag( TileFlags.NorthWall ) )
		{
			var wallRenderer = south.WallObject?.GetComponent<ModelRenderer>();
			if ( wallRenderer != null )
				wallRenderer.Tint = Color.White;
		}

		// West
		var west = board.GetTile( x - 1, y, z );
		if ( west != null && west.HasFlag( TileFlags.EastWall ) )
		{
			var wallRenderer = west.WallObject?.GetComponent<ModelRenderer>();
			if ( wallRenderer != null )
				wallRenderer.Tint = Color.White;
		}

		// East
		var east = board.GetTile( x + 1, y, z );
		if ( east != null && east.HasFlag( TileFlags.WestWall ) )
		{
			var wallRenderer = east.WallObject?.GetComponent<ModelRenderer>();
			if ( wallRenderer != null )
				wallRenderer.Tint = Color.White;
		}
	}

	private void CastRay3D( Vector3Int start, Vector3Int end )
	{
		Tile previous = null;

		foreach ( var tile in Bresenham3D( start.x, start.y, start.z, end.x, end.y, end.z ) )
		{
			if ( tile == null ) break;

			// Stop ray at first solid wall if from the interior
			if ( tile.Wall )
			{
				// Reveal the wall if it’s hit from outside the building
				if ( tile.Z == activeZ && previous != null && previous.BuildingID != 0 )
				{
					previouslyVisible.Add( tile ); // <- Reveal the wall tile
					SetTileVisibility( tile, true, false );
					RevealNeighboringWalls( tile );
				}

				break; // Always stop vision at walls
			}

			// NEW: Block vertical vision through floors/ceilings
			if ( previous != null && tile.Z != previous.Z )
			{
				// If we are going upward or downward and the previous tile has a floor
				if ( previous.Floor ) // You define this flag/property
				{
					break;
				}
			}

			previouslyVisible.Add( tile );
			SetTileVisibility( tile, true, false );
			RevealNeighboringWalls( tile );

			previous = tile;
		}
	}

	public static Vector2 Rotate( Vector2 v, float degrees )
	{
		float radians = MathF.PI / 180f * degrees;
		float cos = MathF.Cos( radians );
		float sin = MathF.Sin( radians );
		return new Vector2(
			v.x * cos - v.y * sin,
			v.x * sin + v.y * cos
		);
	}
}
