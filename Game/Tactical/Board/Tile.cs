using System;
using Sandbox;

public sealed class Tile : Component
{
	public TileFlags Flags { get; set; }

	[Property] public BuildingDefinition BuildingType { get; set; }

	// Bitflag-based tile metadata
	[Property] public bool IsOccupied => Flags.HasFlag( TileFlags.Occupied );
	[Property] public bool FullCover => Flags.HasFlag( TileFlags.FullCover );
	[Property] public bool HalfCover => Flags.HasFlag( TileFlags.HalfCover );
	[Property] public bool Road => Flags.HasFlag( TileFlags.Road );
	[Property] public bool Selected => Flags.HasFlag( TileFlags.Selected );
	[Property] public bool TownCenter => Flags.HasFlag( TileFlags.TownCenter );
	[Property] public bool Generated => Flags.HasFlag( TileFlags.Generated );
	[Property] public bool Floor => Flags.HasFlag( TileFlags.Floor );
	[Property] public bool Door => Flags.HasFlag( TileFlags.Door );
	[Property] public bool Wall => Flags.HasFlag( TileFlags.Wall );
	[Property] public bool Corner => Flags.HasFlag( TileFlags.Corner );
	[Property] public bool WestWall => Flags.HasFlag( TileFlags.WestWall);
	[Property] public bool EastWall => Flags.HasFlag( TileFlags.EastWall );
	[Property] public bool NorthWall => Flags.HasFlag( TileFlags.NorthWall );
	[Property] public bool SouthWall => Flags.HasFlag( TileFlags.SouthWall );

	[Property] public bool Visible => Flags.HasFlag( TileFlags.Visible );

	[Property] public bool NotVisible => Flags.HasFlag( TileFlags.NotVisible );

	[Property] public bool BlocksVision => Flags.HasFlag( TileFlags.BlocksVision );

	[Property] public bool Explored => Flags.HasFlag( TileFlags.Explored );

	[Property] public Unit unit;


	// Grid coordinates (used in A*)
	[Property] public int X { get; set; }
	[Property] public int Y { get; set; }

	[Property] public int Z { get; set; } // Z coordinate for 3D tiles. This will refer to the layer the tile sits on. We'll use a stairway bitflag to indicate if the tile is a stairway tile for astar purposes.

	[Property] public int BuildingID { get; set; }

	[Property] public Unit ReservedBy { get; private set; }

	// World position is needed for unit movement
	public Vector3 WorldPosition => GameObject.WorldPosition;

	// New GameObjects for new worldgen

	[Property] public GameObject WallObject { get; set; }
	[Property] public GameObject DetailObject { get; set; } //Detail objects like furniture and cars.
	[Property] public GameObject FoliageObject { get; set; } //Foliage objects like trees and bushes. Having this seperate from a detail object lets us layer foliage and debris. For example we could add a rock and a bush on the same tile.


	protected override void OnEnabled()
	{



	}

	// Use for checking flags directly
	public bool HasFlag( TileFlags flag )
	{
		return Flags.HasFlag( flag );
	}

	// Modify flags dynamically
	public void SetFlag( TileFlags flag, bool value = true )
	{
		if ( value )
		{
			Flags |= flag;
		}
		else
		{
			Flags &= ~flag;
		}
	}

	// Reset all flags (useful during map regeneration)
	public void ClearFlags()
	{
		Flags = TileFlags.None;
	}

	public void SetUnit(Unit unit)
	{
		if ( unit == null )
		{
			SetFlag( TileFlags.Occupied, false );
			this.unit = null;
			return;
		}
		SetFlag( TileFlags.Occupied, true );
		this.unit = unit;
		

		unit.board = GameObject.Scene.GetAllComponents<Board>().FirstOrDefault(); // Set the board reference
	}

	public void ClearUnit()
	{
		SetFlag( TileFlags.Occupied, false );
		unit = null;
	}	

	public void SetHighlighted()
	{
		this.GameObject.GetComponent<ModelRenderer>().Tint = Color.Yellow;
	}

	public void StopHighlighted()
	{
		var renderer = GameObject.GetComponent<ModelRenderer>();
		renderer.Tint = Color.White;
		if ( renderer == null ) return;

		if ( HasFlag( TileFlags.Visible ) )
			renderer.Tint = Color.White;
		else if ( HasFlag( TileFlags.Explored ) )
			renderer.Tint = Color.Gray;
		else
			renderer.Tint = Color.Black;
	}

	public static int Distance( Tile a, Tile b )
	{
		if ( a == null || b == null )
			return int.MaxValue;

		int dx = Math.Abs( a.X - b.X );
		int dy = Math.Abs( a.Y - b.Y );
		return Math.Max( dx, dy ); // Chebyshev Distance
	}

	

	public bool Reserve( Unit unit )
	{
		if ( ReservedBy == null || ReservedBy == unit )
		{
			ReservedBy = unit;
			return true;
		}
		return false; // Already reserved by someone else
	}

	public void ClearReservation( Unit unit )
	{
		if ( ReservedBy == unit )
			ReservedBy = null;
	}

	public void CopyFlagsFrom( Tile source )
	{
		// Copy raw flags
		this.Flags = source.Flags;

		// Or set flags individually for safety/flexibility:
		this.SetFlag( TileFlags.Wall, source.Wall );
		this.SetFlag( TileFlags.Floor, source.Floor );
		this.SetFlag( TileFlags.Corner, source.Corner );
		this.SetFlag( TileFlags.NorthWall, source.NorthWall);
		this.SetFlag( TileFlags.SouthWall, source.SouthWall );
		this.SetFlag( TileFlags.WestWall, source.WestWall );
		this.SetFlag( TileFlags.EastWall, source.EastWall );
		this.SetFlag( TileFlags.Door, source.Door );
		this.SetFlag( TileFlags.FullCover, source.FullCover );
		this.SetFlag( TileFlags.BlocksVision, source.BlocksVision );
		//this.SetFlag( TileFlags.StairsUp, source.StairsUp );
	}
}
