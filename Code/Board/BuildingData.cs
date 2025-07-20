using System;

public class BuildingDefinition
{
	public string Name { get; set; }
	public int Width { get; set; }
	public int Length { get; set; }

	public string Material { get; set; }
	

	public BuildingDefinition( string name, int width, int length, string material )
	{
		Name = name;
		Width = width;
		Length = length;
		Material = material;
	}



}




public static class BuildingData
{
	public static readonly List<BuildingDefinition> All = new()
	{

		new BuildingDefinition( "House", 8, 6, "tiles/brickpanel.vmat" ),
		new BuildingDefinition( "Farm House", 5, 6, "tiles/roughwood.vmat" ),
		new BuildingDefinition( "Factory", 10, 10, "tiles/brick.vmat" ),
		new BuildingDefinition( "Warehouse", 12, 12, "tiles/sheetmetal.vmat" ),
		new BuildingDefinition( "Office", 8, 10, "tiles/cinderblock.vmat" )
	};

	public static BuildingDefinition GetRandom( Random rand )
	{
		return All[rand.Int( 0, All.Count - 1 )];
	}

}
