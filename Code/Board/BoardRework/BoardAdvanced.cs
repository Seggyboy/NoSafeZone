using System;
using Sandbox;


public sealed class BoardAdvanced : Component
{

	public Dictionary<int, Tile[,]> World = new Dictionary<int, Tile[,]>();
	public List<int> BuildingIDs = new List<int>(); // Store building IDs to avoid duplicates
	public delegate void MapGeneratedEventHandler();

	public event MapGeneratedEventHandler MapGenerated;
	public event MapGeneratedEventHandler MapClosed;
	public bool MapLoaded { get; private set; } = false; // Set to true when the map is loaded and ready for gameplay



	public int TileSize = 100;
	[Property]
	public int Width { get; set; } = 40;
	[Property]
	public int Length { get; set; } = 40;

	protected override void OnEnabled()
	{

	}


	public void GenerateMap()
	{
		if ( MapLoaded )
		{
			Log.Warning( "Map is already generated. Close the map before generating a new one." );
			return;
		}
		World.Clear();
		GenerateFloor(); // Build out layer 0				 //GenerateEntities(); // Build out layer 1
		GenerateRoof(); // Build out layer 3
		PlaceFoliage(); // Build out layer 1
		MapGenerated?.Invoke(); // Notify subscribers that the map has been generated
		MapLoaded = true;
	}

	[Button( "Close Map" )]
	public void CloseMap()
	{
		if ( !MapLoaded )
		{
			Log.Warning( "Map is not generated. This shouldn't be happening." );
			return;
		}
		for ( int x = 0; x < Width; x++ )
		{
			for ( int y = 0; y < Length; y++ )
			{
				for ( int layer = 0; layer < 4; layer++ )
				{
					if ( !World.ContainsKey( layer ) )
						continue;

					var tile = World[layer][x, y];

					if ( tile != null && tile.GameObject != null && tile.GameObject.IsValid )
					{
						tile.GameObject.Destroy();
					}
				}
			}
		}
		World.Clear();
		MapLoaded = false;
		MapClosed?.Invoke();
	}


	public bool IsValidTile( int x, int y )
	{
		return x >= 0 && x < Width && y >= 0 && y < Length;
	}

	public Tile GetTile( int x, int y, int layer = 0 )
	{
		if ( !IsValidTile( x, y ) ) return null;
		if ( !World.ContainsKey( layer ) ) return null;
		return World[layer][x, y];
	}
	public void GenerateFloor()
	{
		World[0] = new Tile[Width, Length];
		Tile[,] floor = World[0];

		for ( int x = 0; x < Width; x++ )
		{
			for ( int y = 0; y < Length; y++ )
			{
				GameObject tileObj = new GameObject();
				var tile = tileObj.AddComponent<Tile>();
				tile.ClearFlags();
				tileObj.WorldPosition = new Vector3( x * TileSize, y * TileSize, 0 ); // Assuming a flat board on the XZ plane
				tile.X = x;
				tile.Y = y;
				var renderer = tileObj.AddComponent<ModelRenderer>();
				renderer.Model = Model.Load( "models/dev/plane.vmdl_c" ); // Replace with your actual model path
				renderer.MaterialOverride = Material.Load( "tiles/sand.vmat" ); // Replace with your actual road material
				tileObj.Name = $"Tile_{x}_{y}";
				floor[x, y] = tile;
			}
		}

		GenerateRoads();
		GenerateBuildings();
		GenerateWalls();
	}

	public void GenerateWalls()
	{

		World[2] = new Tile[Width, Length];
		var wall = World[2];
		var floor = World[0];
		var Random = new Random();



		for ( int x = 0; x < Width; x++ )
		{
			for ( int y = 0; y < Length; y++ )
			{
				var floorTile = floor[x, y];
				if ( !floorTile.Wall && !floorTile.Door ) { continue; }
				GameObject wallTile = new GameObject();
				var tile = wallTile.AddComponent<Tile>();
				World[2][x, y] = tile; // Store the wall tile in the wall layer

				tile.ClearFlags();
				wallTile.WorldPosition = new Vector3( x * TileSize, y * TileSize, 0 ); // Assuming a flat board on the XZ plane
				wallTile.Name = $"WallTile_{x}_{y}";
				var renderer = wallTile.AddComponent<ModelRenderer>();
				renderer.Model = Model.Load( "models/walls/straightwall.vmdl" );
				renderer.MaterialOverride = Material.Load( floorTile.BuildingType.Material ); // Replace with your actual wall material
				Log.Info( floorTile.BuildingType.Material );

				if ( floor[x, y].Door )
				{
					var rend = wallTile.GetComponent<ModelRenderer>();
					rend.Model = Model.Load( "models/walls/doorframe.vmdl" );
					wallTile.Name = $"DoorTile_{x}_{y}";

				}

				if ( floor[x, y].Corner )
				{
					var rend = wallTile.GetComponent<ModelRenderer>();
					rend.Model = Model.Load( "models/walls/cornerwall.vmdl" ); // Use corner wall model
					if ( floor[x, y].Top )
					{



						if ( floor[x, y].Right )
						{
							wallTile.WorldRotation *= Rotation.FromYaw( 270 ); // Rotate to face up
						}

						if ( floor[x, y].Left )
						{
							wallTile.WorldRotation *= Rotation.FromYaw( 180 ); // Rotate to face up
						}
					}

					if ( floor[x, y].Bottom )
					{



						if ( floor[x, y].Right )
						{
							//wallTile.WorldRotation *= Rotation.FromYaw( 270 ); // Rotate to face up
						}

						if ( floor[x, y].Left )
						{
							wallTile.WorldRotation *= Rotation.FromYaw( 90 ); // Rotate to face up
						}
					}
				}


				else
				{
					if ( floor[x, y].Top )
					{
						wallTile.WorldRotation *= Rotation.FromYaw( 180 ); // Rotate to face up
					}



					if ( floor[x, y].Right )
					{
						wallTile.WorldRotation *= Rotation.FromYaw( 270 ); // Rotate to face up
					}

					if ( floor[x, y].Left )
					{
						wallTile.WorldRotation *= Rotation.FromYaw( 90 ); // Rotate to face up
					}



				}


			}
		}
	}

	public void GenerateRoof()
	{
		World[3] = new Tile[Width, Length];
		var roof = World[3];
		var floor = World[0];

		for ( int x = 0; x < Width; x++ )
		{
			for ( int y = 0; y < Length; y++ )
			{
				var floorTile = floor[x, y];
				if ( !floorTile.Floor && !floorTile.Wall ) { continue; }
				GameObject roofTile = new GameObject();
				var tile = roofTile.AddComponent<Tile>();
				World[3][x, y] = tile; // Store the roof tile in the wall layer
				tile.ClearFlags();
				roofTile.WorldPosition = new Vector3( x * TileSize, y * TileSize, 121 ); // Assuming a flat board on the XZ plane
				tile.X = x;
				tile.Y = y;
				var renderer = roofTile.AddComponent<ModelRenderer>();
				renderer.Model = Model.Load( "models/dev/plane.vmdl_c" );
				renderer.MaterialOverride = Material.Load( "tiles/roof.vmat" ); // Replace with your actual wall material
				roofTile.WorldScale = new Vector3( 1, 1, 1 ); // Adjust height as needed
			}
		}
	}

	void GenerateRoads()
	{
		Random Rand = new Random();
		var floor = World[0];
		int RoadIt = Rand.Int( 1, 3 );
		Log.Info( RoadIt + " Roads Generated" );

		for ( int i = 0; i < RoadIt; i++ )
		{
			int roadWidth = Rand.Int( 1, 3 );
			int randX = Rand.Int( 0, Width - 3 );
			int randY = Rand.Int( 0, Length - 3 );
			for ( int x = randX; x < randX + roadWidth; x++ )
			{
				for ( int y = 0; y < Length; y++ )
				{
					var tile = floor[x, y];
					tile.SetFlag( TileFlags.Road, true );
					tile.SetFlag( TileFlags.Generated, true );
					tile.GetComponent<ModelRenderer>().MaterialOverride = Material.Load( "tiles/asphalt.vmat" ); // Replace with your actual road material
				}
				roadWidth = Rand.Int( 2, 3 ); // Random width between 1 and 3 floor
			}

			for ( int y = randY; y < randY + roadWidth; y++ )
			{
				for ( int x = 0; x < Length; x++ )
				{
					var tile = floor[x, y];
					tile.SetFlag( TileFlags.Road, true );
					tile.SetFlag( TileFlags.Generated, true );
					tile.GetComponent<ModelRenderer>().MaterialOverride = Material.Load( "tiles/asphalt.vmat" ); // Replace with your actual road material
				}
				roadWidth = Rand.Int( 1, 3 ); // Random width between 1 and 3 floor
			}
		}


	}

	public void GenerateBuildings()
	{
		// We will place buildings adjacent to road floor. Random buildings will be placed not near road floor, then a a* road will be placed connecting them from the main road to the random building.
		int buildingID = 1;
		for ( int x = 0; x < Width; x++ )
		{
			for ( int y = 0; y < Length; y++ )
			{
				if ( !AdjacentToRoad( x, y ) || AdjacentToBuilding( x, y ) ) { continue; }
				var randBuild = BuildingData.GetRandom( new Random() );
				if ( CanPlaceBuilding( randBuild, x, y ) )
				{
					PlaceBuilding( randBuild, x, y, buildingID );
					buildingID++;
					BuildingIDs.Add( buildingID ); // Store the building ID to avoid duplicates

				}
			}
		}

	}

	public void PlaceBuilding( BuildingDefinition def, int x, int y, int buildingID )
	{

		List<Tile> doorCandidates = new();
		var floor = World[0];

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
					doorCandidates.Add( tile ); // Collect potential door tiles
				}
				if ( isEdge )
				{
					tile.SetFlag( TileFlags.FullCover, true );
					tile.SetFlag( TileFlags.BlocksVision, true );
					tile.SetFlag( TileFlags.Wall, true );
					var renderer = tile.GetComponent<ModelRenderer>();
					renderer.MaterialOverride = Material.Load( "tiles/concrete.vmat" ); // walls


				}
				if ( isLeft )
				{
					tile.SetFlag( TileFlags.Left, true );
				}
				if ( isRight )
				{
					tile.SetFlag( TileFlags.Right, true );
				}
				if ( isTop )
				{
					tile.SetFlag( TileFlags.Top, true );
				}
				if ( isBottom )
				{
					tile.SetFlag( TileFlags.Bottom, true );
				}
				if ( isCorner )
				{
					tile.SetFlag( TileFlags.Corner, true );
				}

				tile.SetFlag( TileFlags.Floor, true );

				tile.GetComponent<ModelRenderer>().MaterialOverride = Material.Load( "tiles/wood.vmat" ); // interior floor


			}
		}

		if ( doorCandidates.Count > 0 )
		{
			var doorTile = doorCandidates[new Random().Next( doorCandidates.Count )];
			doorTile.SetFlag( TileFlags.Door, true );
			doorTile.SetFlag( TileFlags.FullCover, false );
			doorTile.SetFlag( TileFlags.Wall, false );

		}

	}

	public bool CanPlaceBuilding( BuildingDefinition data, int x, int y )
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

	public bool AdjacentToRoad( int x, int y )
	{
		var floor = World[0];
		for ( int dx = -1; dx <= 2; dx++ )
			for ( int dy = -1; dy <= 2; dy++ )
			{
				int tx = x + dx;
				int ty = y + dy;

				if ( tx >= 0 && tx < Width && ty >= 0 && ty < Length )
				{
					if ( floor[tx, ty].Road )
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

		World[1] = new Tile[Width, Length];

		for ( int x = 0; x < Width; x++ )
		{
			for ( int y = 0; y < Length; y++ )
			{
				// 20% chance to place foliage
				if ( rand.Next( 0, 100 ) > 20 || World[0][x, y].Generated )
					continue;

				GameObject foliage = new GameObject();
				foliage.Name = $"Foliage_{x}_{y}";
				var tile = foliage.AddComponent<Tile>();


				tile.ClearFlags();
				tile.SetFlag( TileFlags.HalfCover, true ); // Optional: mark as cover
				World[1][x, y] = tile;
				foliage.WorldPosition = new Vector3( x * TileSize, y * TileSize, 0 );



				var renderer = foliage.AddComponent<ModelRenderer>();

				// Pick a random foliage model from the list
				string selectedModel = foliageModels[rand.Next( foliageModels.Count )];
				renderer.Model = Model.Load( selectedModel );

				var floorTile = World[0][x, y];
				floorTile.tileDetail = foliage;
			}
		}
	}




	public Tile GetWorldTile( Vector2 pos )
	{
		var floor = World[0];
		var x = (int)(pos.x / TileSize);
		var y = (int)(pos.y / TileSize);
		if ( IsValidTile( x, y ) == false )
		{

			return null;
		}
		return floor[x, y];
	}




}
