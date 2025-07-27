public sealed class TownLoader : Component
{
	public List<string> TownNames = new();

	[Button]
	public List<string> LoadTowns()
	{
		if ( !FileSystem.Data.FileExists( "commontowns.json" ) )
		{
			Log.Error( "File not found: commontowns.json" );
			return null;
		}

		var json = FileSystem.Data.ReadAllText( "commontowns.json" );
		var wrapper = Json.Deserialize<TownWrapper>( json );

		if ( wrapper?.Towns != null )
		{
			TownNames = wrapper.Towns;
			return TownNames;
			Log.Info( $"Loaded {TownNames.Count} town names." );
		}
		else
		{
			Log.Error( "Failed to parse town names." );
		}return null;
	}

	// Helper class matching the JSON structure
	public class TownWrapper
	{
		public List<string> Towns { get; set; }
	}
}
