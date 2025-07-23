public class MapGenerator()
{
    private readonly Dictionary<int, Tile[,]> _world;
    private readonly int _width, _length, _height;
    public bool MapLoaded = false;

    public MapGenerator(int width, int length, int height, Dictionary<int, Tile[,]> world)
    {
        _width = width;
        _world = world;
        _length = length;
        _height = height;
    }


    public async Task GenerateMap()
    {
        if (MapLoaded)
        {
            Log.Warning("Map is already generated. Close the map before generating a new one.");
            return;
        }

        GenerateLayer(0);
        GenerateRoads();
        GenerateBuildings();


        for (int layer = 1; layer < 20; layer++)
        {
            Log.Info($"Generating layer {layer}...");
            GenerateLayer(layer);
        }

        PlaceFoliage();
        await Task.Delay(100); // Simulate some delay for map generation, can be adjusted or removed as needed
        TacticalEvents.RaiseMapGenerated(); // Notify subscribers that the map has been generated
        MapLoaded = true;
    }

    public void CloseMap()
    {
        if (!MapLoaded)
        {
            Log.Warning("Map is not generated. This shouldn't be happening.");
            return;
        }
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Length; y++)
            {
                for (int layer = 0; layer < 4; layer++)
                {
                    if (!World.ContainsKey(layer))
                        continue;

                    var tile = World[layer][x, y];

                    if (tile != null && tile.GameObject != null && tile.GameObject.IsValid)
                    {
                        tile.GameObject.Destroy();
                    }
                }
            }
        }
        World.Clear();
        MapLoaded = false;
        TacticalEvents.RaiseMapClosed();
    }

    public void GenerateLayer(int layer)
    {
        World[layer] = new Tile[Width, Length];
        Tile[,] layerDict = World[layer];

        if (layer == 0)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Length; y++)
                {
                    GameObject tileObj = new GameObject();
                    var tile = tileObj.AddComponent<Tile>();
                    tile.ClearFlags();
                    tileObj.WorldPosition = new Vector3(x * GameConstants.Map.TILESIZE, y * GameConstants.Map.TILESIZE); // 121 is the height of each layer, we'll adjust if we increase wallheight.
                    tile.X = x;
                    tile.Y = y;
                    tile.Z = layer;
                    var renderer = tileObj.AddComponent<ModelRenderer>();
                    renderer.Model = Model.Load("models/dev/plane.vmdl_c");
                    renderer.MaterialOverride = Material.Load("tiles/sand.vmat");
                    tileObj.Name = $"Tile_{x}_{y}_{layer}";
                    layerDict[x, y] = tile; // Store the tile in the layer dictionary
                }
            }



        }
        else
        {
            var floorLayer = GetOrCreateLayer(0); // Get the previous layer to copy tiles from
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Length; y++)
                {
                    //Log.Info( $"Generating tile at {x}, {y}, layer {layer} amount of stories are: {floorLayer[x,y].BuildingType.Stories}" );
                    if (floorLayer[x, y].BuildingType == null || layer > floorLayer[x, y].BuildingType.Stories + 1)
                        continue;


                    GameObject tileObj = new GameObject();
                    var tile = tileObj.AddComponent<Tile>();
                    tile.Flags = floorLayer[x, y].Flags;
                    tile.CopyFlagsFrom(floorLayer[x, y]); // Copy flags from the previous layer
                    if (tile.Door)
                    {
                        tile.SetFlag(TileFlags.Door, false);
                        tile.SetFlag(TileFlags.Wall, true);
                    }

                    tileObj.WorldPosition = new Vector3(x * GameConstants.Map.TILESIZE, y * GameConstants.Map.TILESIZE, GameConstants.Map.LAYERHEIGHT * layer); // 121 is the height of each layer, we'll adjust if we increase wallheight.
                    tile.X = x;
                    tile.Y = y;
                    tile.Z = layer;
                    tileObj.Name = $"Tile_{x}_{y}_{layer}";
                    tile.BuildingID = floorLayer[x, y].BuildingID; // Copy building ID from the previous layer
                    tile.BuildingType = floorLayer[x, y].BuildingType; // Copy building type from the previous layer
                    var renderer = tileObj.AddComponent<ModelRenderer>();
                    renderer.Model = Model.Load("models/dev/plane.vmdl_c");
                    renderer.MaterialOverride = Material.Load(tile.BuildingType.FloorMaterial); // Use a different material for the floor
                    if (tile.Wall && layer <= floorLayer[x, y].BuildingType.Stories)
                    {
                        GenerateWall(tile); // Generate walls for the building tile
                    }
                    else if (layer == floorLayer[x, y].BuildingType.Stories + 1) // If it's a floor tile on the last layer of the building
                    {
                        renderer.MaterialOverride = Material.Load("tiles/roof.vmat");
                        tileObj.WorldPosition = new Vector3(x * GameConstants.Map.TILESIZE, y * GameConstants.Map.TILESIZE, (GameConstants.Map.LAYERHEIGHT + 1) * layer); // Adjust height for the roof
                    }

                    layerDict[x, y] = tile; // Store the tile in the layer dictionary





                }
            }
        }




    }

    public void GenerateRoads()
    {
        var floor = GetOrCreateLayer(0);

        // === Primary Horizontal Road with Warping ===
        int horizY = Rand.Int(Length / 2, 2 * Length / 2);
        int horizWidth = Rand.Int(4, 8);
        float horizWarpAmp = Rand.Float(1f, 2f);

        for (int x = 0; x < Width; x++)
        {
            int yOffset = (int)(MathF.Sin(x * 0.15f) * horizWarpAmp); // Slight curve
            for (int w = 0; w < horizWidth; w++)
            {
                int y = horizY + yOffset + w;
                if (IsValidTile(x, y))
                    SetRoadTile(x, y);
            }
        }

        // === Primary Vertical Road with Warping ===
        int vertX = Rand.Int(Width / 2, 2 * Width / 2);
        int vertWidth = Rand.Int(3, 8);
        float vertWarpAmp = Rand.Float(1f, 2f);

        for (int y = 0; y < Length; y++)
        {
            int xOffset = (int)(MathF.Cos(y * 0.15f) * vertWarpAmp);
            for (int w = 0; w < vertWidth; w++)
            {
                int x = vertX + xOffset + w;
                if (IsValidTile(x, y))
                    SetRoadTile(x, y);
            }
        }

        // === Organic Branching Horizontal Roads ===
        for (int y = Rand.Int(3, 10); y < Length; y += Rand.Int(8, 15))
        {

            int branchLength = Rand.Int(8, Width / 2);
            int startX = Rand.Int(0, Width - branchLength);
            int roadWidth = Rand.Int(1, 3);
            int yOffset = Rand.Int(-1, 2); // small curve
            for (int i = 0; i < branchLength; i++)
            {
                int x = startX + i;

                for (int w = 0; w < roadWidth; w++)
                {
                    if (IsValidTile(x, y + yOffset + w))
                        SetRoadTile(x, y + yOffset + w);
                }
            }
        }

        // === Organic Branching Vertical Roads ===
        for (int x = Rand.Int(3, 10); x < Width; x += Rand.Int(8, 15))
        {
            int branchLength = Rand.Int(8, Length / 2);
            int startY = Rand.Int(0, Length - branchLength);
            int xOffset = Rand.Int(-1, 2); // slight meander
            int roadWidth = Rand.Int(1, 3);
            for (int i = 0; i < branchLength; i++)
            {
                int y = startY + i;

                for (int w = 0; w < roadWidth; w++)
                {
                    if (IsValidTile(x + xOffset + w, y))
                        SetRoadTile(x + xOffset + w, y);
                }
            }
        }

        // === Add Dead Ends and Spurs ===
        int numSpurs = Rand.Int(10, 20);
        for (int i = 0; i < numSpurs; i++)
        {
            int startX = Rand.Int(1, Width - 2);
            int startY = Rand.Int(1, Length - 2);
            if (!floor[startX, startY].HasFlag(TileFlags.Road))
                continue;

            int dirX = Rand.Int(-1, 2);
            int dirY = Rand.Int(-1, 2);
            if (dirX == 0 && dirY == 0) continue;

            int len = Rand.Int(3, 8);
            for (int j = 0; j < len; j++)
            {
                int nx = startX + dirX * j;
                int ny = startY + dirY * j;
                if (IsValidTile(nx, ny))
                    SetRoadTile(nx, ny);
            }
        }

        AddRoadDamage();

    }

    void AddRoadDamage(float damageChance = 0.05f)
    {
        var floor = GetOrCreateLayer(0);
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Length; y++)
            {
                var tile = floor[x, y];
                if (tile.HasFlag(TileFlags.Road) && Rand.Float() < damageChance)
                {
                    // Break this road tile
                    tile.SetFlag(TileFlags.Road, false);
                    tile.GetComponent<ModelRenderer>().MaterialOverride = Material.Load("tiles/sand.vmat"); // or just leave it default
                }
            }
        }
    }
    
    private void SetRoadTile( int x, int y )
	{
		var tile = _world[0][x, y];
		tile.SetFlag( TileFlags.Road, true );
		tile.SetFlag( TileFlags.Generated, true );
		var rend = tile.GetComponent<ModelRenderer>();
		rend.MaterialOverride = Material.Load( "tiles/asphalt.vmat" ); // Replace with your actual road material
	}
}