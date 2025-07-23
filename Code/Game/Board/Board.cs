using System;
using System.Threading.Tasks;
using Sandbox;
using static Sandbox.Citizen.CitizenAnimationHelper;


public sealed class Board : Component
{
	
	// random comment
	public List<int> BuildingIDs = new List<int>(); // Store building IDs to sort buildings for logic and such

	public Dictionary<int, Tile[,]> World = new Dictionary<int, Tile[,]>();
	[Property] public int Width, Length, Height;

	Random Rand = new Random(); // having one random object can let us set the seed for testing and debugging purposes.
	
	public bool MapLoaded { get; private set; } = false; // Set to true when the map is loaded and ready for gameplay



	

	protected override void OnEnabled()
	{
	
	}


	public void InitializeMap()
	{
		World.Clear();
		MapGenerator mapGenerator = new MapGenerator(Width, Length, Height, World);
		mapGenerator.GenerateMap();

	}

	


	
	

	public void GenerateWall(Tile tile)
	{

		tile.WallObject = new GameObject(); // Create a new GameObject for the wall tile
		var wallObj = tile.WallObject;
		wallObj.WorldPosition = new Vector3(tile.X * GameConstants.Map.TILESIZE, tile.Y * GameConstants.Map.TILESIZE, GameConstants.Map.LAYERHEIGHT * tile.Z); // Assuming a flat board on the XZ plane
		wallObj.Name = $"WallTile_{tile.X}_{tile.Y}_{tile.Z}";
		var renderer = wallObj.AddComponent<ModelRenderer>();
		renderer.Model = Model.Load("models/walls/straightwall.vmdl"); // Use straight wall model
		renderer.MaterialOverride = Material.Load(tile.BuildingType.WallMaterial); // Use the building's wall material
		AssignRotations(tile, tile.Z);


	}

	public void GenerateDoor( Tile tile )
	{

		tile.WallObject = new GameObject(); // Create a new GameObject for the wall tile
		var wallObj = tile.WallObject;
		wallObj.WorldPosition = new Vector3( tile.X * GameConstants.Map.TILESIZE, tile.Y * GameConstants.Map.TILESIZE, GameConstants.Map.LAYERHEIGHT * tile.Z ); // Assuming a flat board on the XZ plane
		wallObj.Name = $"WallTile_{tile.X}_{tile.Y}_{tile.Z}";
		var renderer = wallObj.AddComponent<ModelRenderer>();
		renderer.Model = Model.Load( "models/walls/doorframe.vmdl" ); // Use straight wall model
		renderer.MaterialOverride = Material.Load( tile.BuildingType.WallMaterial ); // Use the building's wall material
		AssignRotations( tile, tile.Z );


	}

	public void AssignRotations(Tile tile, int layer)
	{

		var wallTile = tile.WallObject;
		var rend = wallTile.GetComponent<ModelRenderer>();
		

		if ( tile.Corner )
		{

			rend.Model = Model.Load( "models/walls/cornerwall.vmdl" ); // Use corner wall model
			if ( tile.NorthWall )
			{



				if ( tile.EastWall )
				{
					wallTile.WorldRotation *= Rotation.FromYaw( 270 ); // Rotate to face up
				}

				if ( tile.WestWall )
				{
					wallTile.WorldRotation *= Rotation.FromYaw( 180 ); // Rotate to face up
				}
			}

			if ( tile.SouthWall )
			{



				if ( tile.EastWall )
				{
					//wallTile.WorldRotation *= Rotation.FromYaw( 270 ); // Rotate to face up
				}

				if ( tile.WestWall )
				{
					wallTile.WorldRotation *= Rotation.FromYaw( 90 ); // Rotate to face up
				}
			}
		}


		else
		{
			if ( tile.NorthWall)
			{
				wallTile.WorldRotation *= Rotation.FromYaw( 180 ); // Rotate to face up
			}



			if ( tile.EastWall )
			{
				wallTile.WorldRotation *= Rotation.FromYaw( 270 ); // Rotate to face up
			}

			if ( tile.WestWall )
			{
				wallTile.WorldRotation *= Rotation.FromYaw( 90 ); // Rotate to face 
			}


			// These should probably be moved into their own method.
			// Done motherfucker!
		}

	}

	public void GenerateRoof(int layer) // This is gonna be rewritten to only run on the final layer for each building. Actually this is fr unneeeded.
	{
		var layerArr = GetOrCreateLayer( layer );

		for ( int x = 0; x < Width; x++ )
		{
			for ( int y = 0; y < Length; y++ )
			{
				var floorTile = layerArr[x,y];
				if ( !floorTile.Floor && !floorTile.Wall ) { continue; }
				GameObject roofTile = new GameObject();
				var tile = roofTile.AddComponent<Tile>();
				tile.ClearFlags();
				roofTile.WorldPosition = new Vector3( x * GameConstants.Map.TILESIZE, y * GameConstants.Map.TILESIZE, 121 ); // Assuming a flat board on the XZ plane
				tile.X = x;
				tile.Y = y;
				var renderer = roofTile.AddComponent<ModelRenderer>();
				renderer.Model = Model.Load( "models/dev/plane.vmdl_c" );
				renderer.MaterialOverride = Material.Load( "tiles/roof.vmat" ); // Replace with your actual wall material
				roofTile.WorldScale = new Vector3( 1, 1, 1 ); // Adjust height as needed
			}
		}
	}

	




	public Tile FindNearestRoad( Tile startTile, int maxRadius = 20 )
	{
		var floor = GetOrCreateLayer(0);

		for ( int radius = 1; radius <= maxRadius; radius++ )
		{
			for ( int dx = -radius; dx <= radius; dx++ )
			{
				for ( int dy = -radius; dy <= radius; dy++ )
				{
					int nx = startTile.X + dx;
					int ny = startTile.Y + dy;

					if ( !IsValidTile( nx, ny ) )
						continue;

					var tile = floor[nx, ny];
					if ( tile.HasFlag( TileFlags.Road ) )
						return tile;
				}
			}
		}

		return null; // No road found within maxRadius
	}

	public List<Tile> GetCardinalNeighbors( int x, int y, int layer )
	{
		List<Tile> neighbors = new();
		var floor = GetOrCreateLayer( layer );	

		if ( IsValidTile( x + 1, y ) ) neighbors.Add( floor[x + 1, y] );
		if ( IsValidTile( x - 1, y ) ) neighbors.Add( floor[x - 1, y] );
		if ( IsValidTile( x, y + 1 ) ) neighbors.Add( floor[x, y + 1] );
		if ( IsValidTile( x, y - 1 ) ) neighbors.Add( floor[x, y - 1] );

		return neighbors;
	}

	

	

public void GenerateBuildings()
	{
		// We will place buildings adjacent to road floor. Random buildings will be placed not near road floor, then a a* road will be placed connecting them from the main road to the random building.
		int buildingID = 1;
		for ( int x = 0; x < Width;x++ )
		{
			for ( int y = 0; y < Length; y++ )
			{
				if (!AdjacentToRoad(x,y) || AdjacentToBuilding(x,y)) { continue; }
				var randBuild = BuildingData.GetRandom( Rand );
				if ( CanPlaceBuilding( randBuild, x, y )  )
				{
					PlaceBuilding( randBuild, x, y, buildingID );
					buildingID++;
					BuildingIDs.Add( buildingID ); // Store the building ID to avoid duplicates

				}
			}
		}

	}

	public void PlaceBuilding(BuildingDefinition def, int x, int y, int buildingID)
	{
		
		List<Tile> doorCandidates = new();
		var floor = World[0];
		//Log.Info( "PLACING BUILDING: " + def.Name + " at " + x + ", " + y );	

		

		for ( int i = 0; i < def.Width; i++ )
		{
			for ( int j = 0; j < def.Length; j++ )
			{
				int tx = x + i;
				int ty = y + j;
				if ( tx < 0 || tx >= Width || ty < 0 || ty >= Length )
					continue;

				var tile = floor[tx, ty];
				tile.BuildingID = buildingID;
				tile.BuildingType = def;
				tile.SetFlag( TileFlags.Generated, true );
				

				// Edge = cover (walls)
				bool isEdge = (i == 0 || j == 0 || i == def.Width - 1 || j == def.Length - 1);
				bool isLeft = (i == 0);
				bool isRight = (i == def.Width - 1);
				bool isTop = (j == 0);
				bool isBottom = (j == def.Length - 1);
				bool onWorldEdge = (tx == 0 || tx == Width - 1 || ty == 0 || ty == Length - 1);

				bool isCorner = (isLeft || isRight) && (isTop || isBottom);

				if ( !isCorner && isEdge && !onWorldEdge )
				{
					foreach ( var neighbor in GetCardinalNeighbors( tx, ty, 0 ) )
					{
						if ( !neighbor.Generated || neighbor.Road )
						{
							doorCandidates.Add( tile );
							break;
						}
					}
				}

				if ( isLeft )
				{
					tile.SetFlag( TileFlags.WestWall, true );
				}
				if (isRight )
				{
					tile.SetFlag( TileFlags.EastWall, true );
				}
				if ( isTop )
				{
					tile.SetFlag( TileFlags.NorthWall, true );
				}
				if ( isBottom )
				{
					tile.SetFlag( TileFlags.SouthWall, true );
				}
				if ( isCorner )
				{
					tile.SetFlag( TileFlags.Corner, true );
				}
				
				tile.SetFlag( TileFlags.Floor, true );
				
				tile.GetComponent<ModelRenderer>().MaterialOverride = Material.Load( "tiles/wood.vmat" ); // interior floor
				if ( isEdge )
				{
					tile.SetFlag( TileFlags.FullCover, true );
					tile.SetFlag( TileFlags.BlocksVision, true );
					tile.SetFlag( TileFlags.Wall, true );



				}
				

			}

			
		}

		if ( doorCandidates.Count > 0 )
		{
			var doorTile = doorCandidates[Rand.Int( 0, doorCandidates.Count - 1 )];

			// Remove existing wall object if it exists
			if ( doorTile.WallObject != null && doorTile.WallObject.IsValid )
			{
				doorTile.WallObject.Destroy();
				doorTile.WallObject = null;
			}

			doorTile.SetFlag( TileFlags.Door, true );
			doorTile.SetFlag( TileFlags.FullCover, false );
			doorTile.SetFlag( TileFlags.Wall, false );
		}

		foreach ( var tile in floor )
		{
			if ( tile == null || tile.BuildingID != buildingID )
				continue;

			if ( tile.Wall && !tile.Door )
			{
				GenerateWall( tile ); // Now it�s safe: doorTile will be skipped
			}
			else if ( tile.Door )
			{
				GenerateDoor( tile );
			}
		}


	}


	/*public void GenerateStoryLayer(BuildingDefinition def, int baseX, int baseY, int story, int absLayer, int layerOffset)
{
	var tiles = GetOrCreateLayer(absLayer);
	var rooms = def.GenerateRooms();

	foreach (var room in rooms)
	{
		for (int dx = 0; dx < room.Width; dx++)
		for (int dy = 0; dy < room.Length; dy++)
		{
			int x = baseX + room.X + dx;
			int y = baseY + room.Y + dy;

			var obj = new GameObject();
			obj.WorldPosition = new Vector3(x * TileSize, y * TileSize, absLayer * 121); // Your FloorHeight
			obj.Name = $"Tile_{x}_{y}_L{absLayer}";

			var tile = obj.AddComponent<Tile>();
			tile.X = x;
			tile.Y = y;

			var rend = obj.AddComponent<ModelRenderer>();
			rend.Model = Model.Load("models/dev/plane.vmdl_c");
			rend.MaterialOverride = Material.Load(def.FloorMaterial);

			tiles[x, y] = tile;

			// Place walls on edge tiles in mid-layer (walls = layer 1 if LayersPerStory == 2)
			bool isWallLayer = (layerOffset == 1);
			if (isWallLayer && (dx == 0 || dy == 0 || dx == room.Width - 1 || dy == room.Length - 1))
			{
				tile.SetFlag(TileFlags.Wall, true);
				var wall = new GameObject();
				wall.WorldPosition = obj.WorldPosition;
				var wallRend = wall.AddComponent<ModelRenderer>();
				wallRend.Model = Model.Load("models/walls/straightwall.vmdl");
				wallRend.MaterialOverride = Material.Load(def.WallMaterial);
				tile.WallObject = wall;
			}

			// Optionally: add stairs at center of room in wall layer
			if (isWallLayer && dx == room.Width / 2 && dy == room.Length / 2 && story < def.Stories - 1)
			{
				tile.SetFlag(TileFlags.StairsUp, true);
				var stairs = new GameObject();
				stairs.WorldPosition = obj.WorldPosition;
				var stairsRend = stairs.AddComponent<ModelRenderer>();
				stairsRend.Model = Model.Load("models/stairs/staircase.vmdl");
				tile.FurnitureObject = stairs;
			}
		}
	}
}
	*/

	public bool CanPlaceBuilding(BuildingDefinition data, int x, int y)
	{
		var floor = World[0];
		if ( x < 0 || y < 0 || x + data.Width > Width || y + data.Length > Length )
			return false;

		for ( int i = 0; i < data.Width; i++ )
		{
			for ( int j = 0; j < data.Length; j++ )
			{
				var tile = floor[x + i, y + j];

				// This checks if the tile is already used, or is part of a road
				if ( tile.Generated || tile.Road )
					return false;
			}
		}
		return true;
	}

	public bool AdjacentToRoad(int x, int y)
	{
		var floor = World[0];
		for ( int dx = -1; dx <= 2; dx++ )
			for ( int dy = -1; dy <= 2; dy++ )
			{
				int tx = x + dx;
				int ty = y + dy;

				if ( tx >= 0 && tx < Width && ty >= 0 && ty < Length )
				{
					if ( floor[tx, ty].Road)
						return true;
				}
			}
		return false;
	}

	public bool AdjacentToBuilding( int x, int y )
	{
		var floor = World[0];
		for ( int dx = -1; dx <= 2; dx++ )
			for ( int dy = -1; dy <= 2; dy++ )
			{
				int tx = x + dx;
				int ty = y + dy;
				if ( tx >= 0 && tx < Width && ty >= 0 && ty < Length )
				{
					if ( floor[tx, ty].Wall )
						return true;
				}
			}
		return false;
	}


	public void PlaceFoliage()
	{
		Random rand = new Random();

		// List of foliage model paths
		List<string> foliageModels = new List<string>
	{
		"models/rust_nature/overgrowth/bush_large_dense.vmdl_c",
		"models/rust_nature/overgrowth/bush_medium_dense.vmdl_c",
		"models/rust_nature/overgrowth/bush_sparse.vmdl_c",
	};


		for ( int x = 0; x < Width; x++ )
		{
			for ( int y = 0; y < Length; y++ )
			{
				// 20% chance to place foliage
				if ( rand.Next( 0, 100 ) > 20 || World[0][x, y].Generated )
					continue;

				GameObject foliage = new GameObject();
				foliage.Name = $"Foliage_{x}_{y}";

				World[0][x, y].SetFlag( TileFlags.HalfCover, true );
				foliage.WorldPosition = new Vector3( x * GameConstants.Map.TILESIZE, y * GameConstants.Map.TILESIZE, 0 );



				var renderer = foliage.AddComponent<ModelRenderer>();

				// Pick a random foliage model from the list
				string selectedModel = foliageModels[rand.Next( foliageModels.Count )];
				renderer.Model = Model.Load( selectedModel );

				var floorTile = World[0][x, y];
				floorTile.FoliageObject = foliage;
			}
		}
	}




	public Tile GetWorldTile( Vector2 pos )
	{
		var floor = World[0];
		var x = (int)(pos.x / GameConstants.Map.TILESIZE);	
		var y = (int)(pos.y / GameConstants.Map.TILESIZE);	
		if(IsValidTile( x, y ) == false )
		{
			
			return null;
		}
		return floor[x, y];
	}

	public Tile[,] GetOrCreateLayer( int layer )
	{
		if ( !World.ContainsKey( layer ) )
		{
			World[layer] = new Tile[Width, Length];
		}
		return World[layer];
	}

	public bool IsValidTile( int x, int y)
	{
		return x >= 0 && x < Width && y >= 0 && y < Length;
	}

	public Tile GetTile( int x, int y, int z )
	{
		if ( !IsValidTile( x, y ) ) return null;
		if ( !World.ContainsKey( z ) ) return null;
		return World[z][x, y];
	}


}
