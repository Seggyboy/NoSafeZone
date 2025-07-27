[System.Serializable]
public class RegionNode
{
	[Property] public string Name { get; set; }
	[Property] public Vector2 Position { get; set; }
	[Property] public string Biome { get; set; } = "";
	[Property] public List<int> Neighbors { get; set; } = new();
	[Property] public bool Explored { get; set; } = false;
	[Property] public bool IsOccupied { get; set; } = false;
	[Property] public int Index { get; set; } = -1; // Index in the Nodes list of the TravelerMap component
}
