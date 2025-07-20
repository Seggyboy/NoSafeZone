public enum TileFlagsAdvanced
{
	None = 0,
	Occupied = 1 << 0,   // 00001
	FullCover = 1 << 1,  // 00010
	HalfCover = 1 << 2,  // 00100
	Road = 1 << 3,       // 01000
	Selected = 1 << 4,   // 10000
	TownCenter = 1 << 5,
	Generated = 1 << 6,
	Floor = 1 << 7,
	Door = 1 << 8,
	Wall = 1 << 9,
	Corner = 1 <<10,
	Left = 1 << 11,
	Right = 1 <<12,
	Top = 1 << 13,
	Bottom = 1 << 14,	
	Visible = 1 << 15, 
	NotVisible =1 << 16, 
	BlocksVision = 1 << 17,
	Explored = 1 << 18, // Used for fog of war


}

public enum TileLayer
{
	Floor = 0,
	Furniture = 1,
	Wall = 2,
	Roof = 3
}
