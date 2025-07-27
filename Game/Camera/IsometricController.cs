using System;
using Sandbox;

public sealed class IsometricController : Component
{
	[Property] public CameraComponent Camera { get; set; }
	[Property] public BattleMaster battleMaster { get; set; }

	[Property] public FogOfWar FogOfWarController { get; set; }

	public PlayerUnits playerUnits { get; set; }

	[Property] public bool PlayersTurn = false;
	[Property] Board Board { get; set; }

	[Property] public Unit HighLightedUnit;
	[Property] public Unit SelectedUnit;
	[Property] public Unit TargetUnit;

	[Property] public Tile lastHighlightedTile;
	[Property] public Tile SelectedTile;
	

	[Property] public bool ControllerActive = false;
	[Property] PanelComponent tacticalMapHud { get; set; } // Assuming you have a TacticalMapHud component to manage the tactical map UI
	private int _selectedZ = 0;


	[Property]
	public int SelectedZ
	{
		get => _selectedZ;
		set
		{
			if ( _selectedZ != value )
			{
				_selectedZ = value;

				TacticalEvents.RaiseChangedLayerZEvent(value); // Trigger event if changed
				
			}
		}
	}

	

	[Property] private float zoom = 50f;
	public float PanSpeed = 1200f;
	public float ZoomSpeed = 5f;
	public float MinZoom = 0.1f;
	public float MaxZoom = 1f;
	public float MaxOrthographicHeight;
	public Tile ActiveTile;

	public ControllerStates State = ControllerStates.MoveAction; // Default state is MoveAction






	protected override void OnEnabled()
	{
		Mouse.Visibility = MouseVisibility.Visible;
		
		Camera = Scene.GetAllComponents<CameraComponent>().FirstOrDefault();
		battleMaster = Scene.GetAllComponents<BattleMaster>().FirstOrDefault();
		playerUnits = Scene.GetAllComponents<PlayerUnits>().FirstOrDefault();	

		TacticalEvents.StartPlayerTurnEvent += () =>
		{
			PlayersTurn = true;
			Log.Info( "Player's turn started." );
	

		};

		TacticalEvents.StartAITurnEvent += () =>
		{
			PlayersTurn = false;
		};


		TacticalEvents.MapGeneratedEvent += () =>
		{
			ControllerActive = true;
		};

		TacticalEvents.MapClosedEvent += () =>
		{
			ControllerActive = false;
			Clear();
			
		};

		

		MaxOrthographicHeight = 3500;
	}
	protected override void OnFixedUpdate()
	{
		if (!ControllerActive)
		{
			return; // Skip update if controller is not active
		}
		HandlePan();
		HandleZoom();
		UpdateLayerVisibility();
		//HandleUI(); // Handle UI Interactions
		if ( !PlayersTurn )
		{
			return; // Don't process input if it's not the player's turn
		}

		switch(State)
		{
			case
				ControllerStates.MoveAction:
				HandleMoveAction();
				break;
			case
				ControllerStates.AttackAction:
				HandleAttackAction();
				break;
			case
				ControllerStates.MenuAction:
				HandleMenuAction();
				break;

		}
		




	}

	void UpdateLayerVisibility()
	{

		FogOfWarController.SetVisibility();
	}

	public void HandleMoveAction()
	{
		ActiveTile = GetActiveTile();
		if ( ActiveTile == null )
		{
			return;
		}
		if ( ActiveTile.IsOccupied && ActiveTile.unit.Team != UnitTeam.AI )
		{
			HighLightedUnit = ActiveTile.unit;
			if ( Input.Down( "Attack1" ) && HighLightedUnit.Team == UnitTeam.Player )
			{
				SelectedUnit = ActiveTile.unit;
			}
			if ( Input.Down( "Attack2" ) )
			{
				SelectedUnit = null;
			}

		}

		{
			if ( lastHighlightedTile != null && lastHighlightedTile != ActiveTile )
			{
				lastHighlightedTile.StopHighlighted();
			}
			if ( ActiveTile != null )
			{
				ActiveTile.SetHighlighted();
			}

			lastHighlightedTile = ActiveTile;
			SelectedTile = ActiveTile;
			if ( Input.Down( "Attack1" ) )
			{
				if ( SelectedUnit != null && SelectedTile != null )
				{
					SelectedUnit.MoveTo( SelectedTile );
				}
			}
			if ( Input.Down( "Attack2" ) )
			{
				if ( SelectedUnit != null && SelectedTile != null )
				{
					SelectedUnit.FaceTile( SelectedTile, false );
				}
			}
			HighLightedUnit = null;
		}

	}

	public void HandleAttackAction()
	{

		ActiveTile = GetActiveTile();
		if ( ActiveTile == null )
		{
			return;
		}
		if ( ActiveTile.IsOccupied )
		{
			HighLightedUnit = ActiveTile.unit;
			if ( Input.Down( "Attack1" ) && HighLightedUnit.Team == UnitTeam.AI )
			{
				TargetUnit = ActiveTile.unit;
			}
			else if (Input.Down ("Attack1") && HighLightedUnit.Team == UnitTeam.Player)
			{
				SelectedUnit = ActiveTile.unit; // Select the unit to attack
			}
			if ( Input.Down( "Attack2" ) )
			{
				TargetUnit = null;
			}

		}

		{
			if ( lastHighlightedTile != null && lastHighlightedTile != ActiveTile )
			{
				lastHighlightedTile.StopHighlighted();
			}
			if ( ActiveTile != null )
			{
				ActiveTile.SetHighlighted();
			}

			lastHighlightedTile = ActiveTile;
			SelectedTile = ActiveTile;
			
			// Click UI to confirm firing
		}
	}

	public void HandleMenuAction()
	{
		throw new NotImplementedException( "Menu action handling is not implemented yet." );
	}

	public Tile GetActiveTile()
	{
		if ( Camera is null || Board is null )
		{
			Log.Warning( "Camera or Board is null." );
			return null;
		}

	

		Ray ray = Camera.ScreenPixelToRay( Mouse.Position );
		var plane = new Plane( Vector3.Up, 0f );
		var traceResult = plane.Trace( ray );

		if ( !traceResult.HasValue )
		{
			return null;
		}

		Vector3 hit = traceResult.Value;

		// Snap world position to the center of the nearest tile
		float snappedX = MathF.Floor( hit.x / GameConstants.Map.TILESIZE ) * GameConstants.Map.TILESIZE + GameConstants.Map.TILESIZE / 2f;
		float snappedY = MathF.Floor( hit.y / GameConstants.Map.TILESIZE ) * GameConstants.Map.TILESIZE + GameConstants.Map.TILESIZE / 2f;

		Vector2 snappedTilePos = new Vector2( snappedX, snappedY );
		var tile = Board.GetWorldTile( snappedTilePos );
		if ( tile != null )
		{
			return tile;
		}
		return null;
	}

	private void HandlePan()
	{
		Vector3 move = Vector3.Zero;

		if ( Input.Down( "Forward" ) ) move += Camera.WorldRotation.Up.Normal;
		if ( Input.Down( "Backward" ) ) move -= Camera.WorldRotation.Up.Normal;
		if ( Input.Down( "Right" ) ) move += Camera.WorldRotation.Right.WithY( 0 ).Normal;
		if ( Input.Down( "Left" ) ) move -= Camera.WorldRotation.Right.WithY( 0 ).Normal;
		if (Input.Down( "Run" ) ) move *= 2f; // Double speed when running

		if ( move.Length > 0 )
		{
			Camera.WorldPosition += move * PanSpeed * Time.Delta;
		}
	}

	private void HandleZoom()
	{
		float scroll = Input.MouseWheel.y;
		zoom = Math.Clamp( zoom - scroll * ZoomSpeed * Time.Delta, MinZoom, MaxZoom );
		Camera.OrthographicHeight = MaxOrthographicHeight * zoom;

	}

	[Button( "End Turn" )]
	public void EndTurn()
	{
		if ( !PlayersTurn )
		{
			Log.Info( "It's not your turn!" );
			return;
		}
		

		playerUnits.EndTurn();
		PlayersTurn = false;
		SelectedUnit = null;
		Clear();
	}
	

	public void Clear()
	{
		HighLightedUnit = null;
		lastHighlightedTile = null;
		SelectedUnit = null;
		SelectedTile = null;
	}

	public void StartAttack()
	{
		State = ControllerStates.AttackAction;
	}

	public void StartMove()
	{
		State = ControllerStates.MoveAction;
	}	

	public void StartMenu()
	{
		State = ControllerStates.MenuAction;
	}
}


