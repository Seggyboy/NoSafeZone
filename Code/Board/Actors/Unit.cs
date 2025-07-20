using Sandbox;
using System;
using System.Collections.Generic;

public class Unit : Component
{
	[Property] public Board board { get; set; }

	[Property] public FogOfWar fogOfWar { get; set; }

	[Property] public int Health = 100;

	[Property] private int MaxHealth = 100;

	[Property] public int Panic = 0; // 0-100, 100 being the most panicked

	[Property] public int Aim = 0; // 0-10


	[Property] public int TimeUnits = 0;

	[Property] public int MaxTimeUnits = 0; // Maximum Time Units this unit can have, can be set in editor

	[Property] private SkillSet SkillSet = new SkillSet();
	[Property] public UnitTeam Team { get; set; } = UnitTeam.Player;
	[Property] public Tile DestTile { get; set; } // Set in editor
	[Property] public Tile CurrentTile { get; set; } // Set at runtime or in editor
	[Property] public Vector3 DestAngle { get; set; }

	[Property] public Tile TargetTile { get; set; } // Tile this unit is currently facing, used for movement and shooting logic

	[Property] public Rotation TargetRotation { get; set; }

	
	[Property] public List<Tile> Path { get; set; }

	[Property] public UnitState unitState = UnitState.Idle;

	[Property] public int VisibleRange = 10; // Range of visibility for this unit, can be set in editor

	[Property] public int FOV { get; set; } = 120;

	[Property] public int VisionRadius { get; set; } = 20;

	[Property] public Vector3 Forward;

	[Property] SkinnedModelRenderer modelRenderer { get; set; }

	[Property] public Weapon weapon { get; set; }

	private bool awaitingNextTile = false;

	public float ChangeAngleTimer { get; set; } = 0f;

	private int CostPerTile = 2;
	private int CostPerTurn = 1;
	private int CostPerShot = 3;
	[Property] private bool hasPaidTurnCost = false;


	
	



	private int currentPathIndex = 0;
	private float moveSpeed = 400f;

	protected override void OnEnabled()
	{

		Forward = GameObject.WorldRotation.Forward.WithZ( 0 ).Normal;
		
		board = Scene.GetAllComponents<Board>().FirstOrDefault();
		fogOfWar = Scene.GetAllComponents<FogOfWar>().FirstOrDefault();

		SetAttributes();
	
		//ModelRenderer = this.GameObject.GetComponent<SkinnedModelRenderer>();
	}

	protected override void OnUpdate()
	{
		
		//modelRenderer = this.GameObject.GetComponent<SkinnedModelRenderer>();
		//modelRenderer.Sequence.Name = "Idle";
		var go = GameObject;

		if ( awaitingNextTile && unitState == UnitState.Idle )
		{
			awaitingNextTile = false;
			SetNextTile(); // This will reenter Turning or Moving
		}

		if ( unitState == UnitState.Turning )
		{
			if(TimeUnits - CostPerTurn < 0 )
			{
				//Log.Info( "Not enough Time Units to turn." );
				unitState = UnitState.Idle;
				return;
			}
			Forward = GameObject.WorldRotation.Forward.WithZ( 0 ).Normal;
			go.WorldRotation = go.WorldRotation.LerpTo( TargetRotation, 5f * Time.Delta );
			//modelRenderer.Sequence.Name = "Walk"; 
			//modelRenderer.Sequence.Time += Time.Delta;

			if ( go.WorldRotation.Forward.WithZ( 0 ).Dot( TargetRotation.Forward.WithZ( 0 ) ) >= 0.999f )
			{
				if ( !hasPaidTurnCost && TimeUnits >= CostPerTurn )
				{
					ChangeAngleTimer = 0f;
					DeductTUs( CostPerTurn );
					if( Team == UnitTeam.Player)
					{
						fogOfWar?.SetVisibility();
					}
					
					hasPaidTurnCost = true;
				}
				else if ( !hasPaidTurnCost && TimeUnits < CostPerTurn )
				{
					//Log.Info( "Not enough Time Units to complete the turn." );
					unitState = UnitState.Idle;
					
				}

				

				if ( Path != null && Path.Count > 1 && TimeUnits > CostPerTile)
				{
					unitState = UnitState.Moving;
				}
				else
				{
					unitState = UnitState.Idle;
				}
			}
			else if( ChangeAngleTimer > 3f )
			{
				// If turning takes too long, reset state
				ChangeAngleTimer = 0f;
				//Log.Info( "Turning took too long, resetting state." );
				unitState = UnitState.Idle;
				DeductTUs( CostPerTurn );
				
				
			}
			else
			{
				ChangeAngleTimer += Time.Delta;
			}


		}

		else if ( unitState == UnitState.Moving )
		{
			if ( TimeUnits < CostPerTile )
			{
				//Log.Info( "Movement halted: not enough Time Units." );
				unitState = UnitState.Idle;
				return;
			}
			//modelRenderer.Sequence.Name = "Walk";
			Forward = GameObject.WorldRotation.Forward.WithZ( 0 ).Normal;

			Vector3 moveDir = (DestTile.WorldPosition - go.WorldPosition).Normal;

			float distance = (DestTile.WorldPosition - go.WorldPosition).Length;

			// Move toward the destination using constant speed
			Vector3 newPos = go.WorldPosition + moveDir * moveSpeed * Time.Delta;

			// Clamp to destination if we overshoot
			if ( (DestTile.WorldPosition - newPos).Dot( moveDir ) <= 0f )
			{


				DeductTUs( CostPerTile );
				if( Team == UnitTeam.Player )
				{
					fogOfWar?.SetVisibility();
				}
			
				newPos = DestTile.WorldPosition;
				CurrentTile.SetFlag( TileFlags.Occupied, false );
				CurrentTile.ClearUnit();
				CurrentTile.ClearReservation(this);
				CurrentTile = DestTile;
				DestTile.SetFlag( TileFlags.Occupied, true );
				CurrentTile.SetUnit( this );
				currentPathIndex++;
				if ( currentPathIndex >= Path.Count )
				{
					unitState = UnitState.Idle;
					Path = null;
					DestTile = null;
					
				}
				else
				{
					awaitingNextTile = true;
					unitState = UnitState.Idle;
				}

				if ( CurrentTile != null )
				{
					CurrentTile.ClearReservation( this );
				}

				

			}

			go.WorldPosition = newPos;
			return;
		}
		
	}

	public void SetAttributes()
	{
		Health = (int)(100f + SkillSet.GetSkillLevel( SkillTypes.Vitality ) * 1.25f); // Base health + skill bonus
		MaxHealth = Health; // Max health is set to current health for simplicity
		
		TimeUnits = (int) ( 40 + SkillSet.GetSkillLevel(SkillTypes.Agility ) * 1.25); // Base Time Units + skill bonus
		MaxTimeUnits = TimeUnits;
		Panic = 0; // Reset panic for new turn
		Aim = (int)(SkillSet.GetSkillLevel( SkillTypes.Aim )); // Aim skill level, can be used for shooting mechanics
		VisibleRange = (int) (10 + SkillSet.GetSkillLevel( SkillTypes.Perception ) * 1.25); // Base visible range + skill bonus
	}

	public virtual void MoveTo( Tile destinationTile )
	{
		if ( destinationTile == null || destinationTile == CurrentTile )
			return;
		TargetTile = null;
		// Clear current movement state
		unitState = UnitState.Idle;
		Path = null;
		DestTile = null;
		currentPathIndex = 0;
		TargetTile = destinationTile;

		// Request new path
		var newPath = AStarManager.Instance?.RequestPath( CurrentTile, destinationTile, false );

		if ( newPath != null && newPath.Count > 1 )
		{
			Path = newPath;
			currentPathIndex = 1; // Start at 1 to avoid moving "back" to current tile
			SetNextTile();
		}
		else
		{
			Log.Warning( "No path found or destination unreachable." );
		}
	}


	public void SetTile(int x, int y)
	{
		var tile = board.World[0][x, y];
		if ( tile == null )
		{
			Log.Warning( "Cannot set a null tile." );
			return;
		}
		if ( CurrentTile != null )
		{
			CurrentTile.SetFlag( TileFlags.Occupied, false );
			CurrentTile.ClearUnit();
		}
		CurrentTile = tile;
		CurrentTile.SetFlag( TileFlags.Occupied, true );
		CurrentTile.SetUnit( this );

		GameObject.WorldPosition = CurrentTile.WorldPosition;
		unitState = UnitState.Idle;
	}

	public void SetTile(Tile tile )
	{
		
		if ( tile == null )
		{
			Log.Warning( "Cannot set a null tile." );
			return;
		}
		if ( CurrentTile != null )
		{
			CurrentTile.SetFlag( TileFlags.Occupied, false );
			CurrentTile.ClearUnit();
		}
		CurrentTile = tile;
		CurrentTile.SetFlag( TileFlags.Occupied, true );
		CurrentTile.SetUnit( this );

		GameObject.WorldPosition = CurrentTile.WorldPosition;
		unitState = UnitState.Idle;
	}

	public virtual void SetNextTile()
	{
		DestTile = Path[currentPathIndex];
		// When validating each tile in the path:
		if ( DestTile.IsOccupied || (DestTile.ReservedBy != null && DestTile.ReservedBy != this) )
		{
			// Conflict: tile is already claimed
			Log.Info( $"Path blocked at {DestTile.X}, {DestTile.Y} — rebuilding." );
			RebuildPathWithAvoidance();
			return;
		}

		if (Team != UnitTeam.Player )
		{
			DestTile.Reserve( this ); // Claim the tile you're moving into
		}
		


		if ( currentPathIndex >= Path.Count )
		{
			unitState = UnitState.Idle;
			Path = null;
			DestTile = null;
			return;
		}

		if ( DestTile.HasFlag( TileFlags.Occupied ) )
		{
			unitState = UnitState.Idle;
			//Log.Info( $"Destination tile {DestTile.X},{DestTile.Y} is occupied. Aborting path." );
			Path = null;
			DestTile = null;
			return;
		}

		if ( TimeUnits < CostPerTile )
		{
			unitState = UnitState.Idle;
			//Log.Info( "Not enough Time Units to continue moving." );
			return;
		}

		

		Vector3 direction = (DestTile.WorldPosition - GameObject.WorldPosition).Normal;

		// Only pay turn cost if direction change is significant

		if ( TimeUnits < CostPerTurn )
		{
			unitState = UnitState.Idle;
			//Log.Info( "Not enough Time Units to turn." );
			return;
		}

		Face( direction, true );
	}

	public void RebuildPathWithAvoidance()
	{
		if ( TargetTile == null || CurrentTile == null ) return;

		// Use A* that skips Reserved or Occupied tiles
		var newPath = AStarManager.Instance?.RequestPath( CurrentTile, TargetTile, true );

		if ( newPath != null && newPath.Count > 1 )
		{
			Path = newPath;
			currentPathIndex = 1;
			SetNextTile();
		}
		else
		{
			Log.Warning( $"{GameObject.Name} could not find a clear path." );
			Path = null;
			unitState = UnitState.Idle;
		}
	}

	public void Face( Vector3 direction, bool forMovement )
	{
		if ( direction.Length.AlmostEqual( 0f ) ) return;

		var desiredRotation = Rotation.LookAt( direction, Vector3.Up );

		// Only turn if there's a meaningful difference
		if ( GameObject.WorldRotation.AlmostEqual( desiredRotation, 0.01f ) && forMovement )
		{
			// Already facing that way, just go straight to moving
			unitState = UnitState.Moving;
			return;
		}

		TargetRotation = desiredRotation;
		hasPaidTurnCost = false;
		unitState = UnitState.Turning;
	}

	public void FaceTile( Tile tile, bool forMovement )
	{
		if ( tile == null )
		{
			Log.Warning( "Cannot face a null tile." );
			return;
		}
		

		Vector3 direction = (tile.WorldPosition - GameObject.WorldPosition).Normal;
		Face( direction, forMovement );
	}

	[Button( "Move To Tile" )]
	public void MoveToTile()
	{
		if ( DestTile != null )
		{
			MoveTo( DestTile );
		}
		else
		{
			Log.Warning( "Destination tile is not set." );
		}
	}

	public void DeductTUs( int amount )
	{
		if ( amount <= 0 ) return;
		TimeUnits = Math.Clamp( TimeUnits - amount, 0, 120 );
		//Log.Info( $"{GameObject.Name} spent {amount} TU. Before: {TimeUnits}" + "Their state was: " + unitState);
		

	}

	// Shooting Logic
	public void Shoot( Tile targetTile )
	{
		FaceTile( targetTile,false );
		if ( weapon == null )
		{
			Log.Warning( "No weapon equipped." );
			return;
		}
		if ( TimeUnits < CostPerShot )
		{
			Log.Warning( "Not enough Time Units to shoot." );
			return;
		}
		weapon.Shoot(CurrentTile, targetTile, Aim, Panic );
		DeductTUs( CostPerShot );


	}

	public void Hurt(int amount )
	{
		if ( amount <= 0 ) return;
		Health = Math.Clamp( Health - amount, 0, MaxHealth );
		if ( Health <= 0 )
		{
			//Log.Info( $"{GameObject.Name} has been defeated!" );
			Kill();
		}
		else
		{
			Log.Info( $"{GameObject.Name} took {amount} damage. Remaining health: {Health}/{MaxHealth}" );
		}
	}

	public void Kill() // Graceful exit from the map.
	{
		Log.Info( $"{GameObject.Name} has been killed!" );
		GameObject.Destroy();
		CurrentTile.SetFlag( TileFlags.Occupied, false );
		CurrentTile.ClearUnit();
		if ( Path != null )
		{
			Path.Clear();
			Path = null;
		}
		unitState = UnitState.Idle;
		DestTile = null;
		currentPathIndex = 0;
		var battleMaster = BattleMaster.Instance;
		if (battleMaster != null)
		{
			battleMaster.PlayerUnitsList.Remove( GameObject );
			battleMaster.OpposingUnitsList.Remove( GameObject );
		}
		
	}
}



public enum UnitTeam
{
	Player,
	AI
}

public enum UnitState
{
	Idle,
	Turning,
	Moving,
	Attacking
}
