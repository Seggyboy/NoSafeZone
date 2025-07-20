using System;
using Sandbox;

public sealed class TileAdvanced : Component
{
	public TileFlags Flags { get; private set; }

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
	[Property] public bool Left => Flags.HasFlag( TileFlags.Left );
	[Property] public bool Right => Flags.HasFlag( TileFlags.Right );
	[Property] public bool Top => Flags.HasFlag( TileFlags.Top );
	[Property] public bool Bottom => Flags.HasFlag( TileFlags.Bottom );

	[Property] public bool Visible => Flags.HasFlag( TileFlags.Visible );

	[Property] public bool NotVisible => Flags.HasFlag( TileFlags.NotVisible );

	[Property] public bool BlocksVision => Flags.HasFlag( TileFlags.BlocksVision );

	[Property] public bool Explored => Flags.HasFlag( TileFlags.Explored );



	[Property] public Unit unit;

	[Property] public GameObject FloorObject;
	[Property] public GameObject WallObject;
	[Property] public List<GameObject> FurnitureObjects = new();
	[Property] public GameObject RoofObject;

	// Grid coordinates (used in A*)
	[Property] public int X { get; set; }
	[Property] public int Y { get; set; }
	[Property] public int Z { get; set; }


	[Property] public int BuildingID { get; set; }

	// World position is needed for unit movement
	public Vector3 WorldPosition => GameObject.WorldPosition;




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
}


