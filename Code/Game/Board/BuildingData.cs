using System;
using static Sandbox.Citizen.CitizenAnimationHelper;

public class BuildingDefinition
{
	public string Name;
	public int Width;
	public int Length;
	public int Stories;
	public int LayersPerStory = 2;
	public bool HasRoof;

	public string WallMaterial;
	public string FloorMaterial;

	public Func<RoomBlueprint[]> GenerateRooms;

	public BuildingDefinition( string name, int width, int length, int stories, string wallMat, string floorMat, bool hasRoof = true )
	{
		Name = name;
		Width = width;
		Length = length;
		Stories = stories;
		WallMaterial = wallMat;
		FloorMaterial = floorMat;
		HasRoof = hasRoof;

		// Default: one big room
		GenerateRooms = () =>
		{
			return new RoomBlueprint[]
			{
				new RoomBlueprint { X = 0, Y = 0, Width = width, Length = length }
			};
		};
	}
}

public class RoomBlueprint
{
	public int X;
	public int Y;
	public int Width;
	public int Length;
}



public static class BuildingData
{
	public static readonly List<BuildingDefinition> All = new()
	{

		new BuildingDefinition( "One Story House", 8, 6, 1, "tiles/brickpanel.vmat", "tiles/wood.vmat", true ),
		new BuildingDefinition( "Farm House", 5, 6, 1, "tiles/roughwood.vmat" , "tiles/wood.vmat", true),
		new BuildingDefinition( "Factory", 10, 10, 2, "tiles/brick.vmat", "tiles/wood.vmat", true ),
		new BuildingDefinition( "Warehouse", 12, 12, 5, "tiles/sheetmetal.vmat", "tiles/wood.vmat", true ),
		new BuildingDefinition( "Office", 8, 10, 3, "tiles/cinderblock.vmat", "tiles/wood.vmat", true)
	};

	public static BuildingDefinition GetRandom( Random rand )
	{

		if ( rand == null )
		{
			Log.Error( "GetRandom called with null Random — using fallback Rand." );
			rand = new Random(); // use Sandbox’s global RNG or your own shared one
		}

		var randChoice = All[rand.Int( 0, All.Count - 1 )];
		//Log.Info( $"Randomly selected building: {randChoice.Name}" );
		return randChoice;
	}

}
