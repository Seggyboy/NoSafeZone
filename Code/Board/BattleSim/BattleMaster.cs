using System;
using Sandbox;

public sealed class BattleMaster : Component
{
	[Property] private Board board;
	[Property] private OpposingUnits opposingUnits;
	[Property] private PlayerUnits playerUnits;
	[Property] private AStarPathFinder pathFinder;

	public delegate void StartTurnEventHandler();
	public event StartTurnEventHandler StartPlayerTurnEvent;
	public event StartTurnEventHandler StartAITurnEvent;	
	public event Action WinConditionMetEvent;
	public event Action LossConditionMetEvent;

	public delegate void LevelLoadedEventHandler();
	public event LevelLoadedEventHandler LevelLoadedEvent;

	[Property] public int ZombieCount => OpposingUnitsList.Count;
	[Property] public int PlayerUnitCount => PlayerUnitsList.Count;

	[Property] public List<GameObject> PlayerUnitsList = new List<GameObject>();
	[Property] public List<GameObject> OpposingUnitsList = new List<GameObject>();

	public static BattleMaster Instance { get; private set; }




	public int TurnNumber { get; private set; } = 0;


	public bool WinConditionsMet { get; private set; } = false;
	protected override void OnEnabled()
	{
		Instance = this;
		board = Scene.GetAllComponents<Board>().FirstOrDefault();
		opposingUnits = Scene.GetAllComponents<OpposingUnits>().FirstOrDefault();
		playerUnits = Scene.GetAllComponents<PlayerUnits>().FirstOrDefault();
		board.MapGenerated += () =>
		{
			InitAStar();
			OnMapReady(); // Move this inside the lambda
		};
		board.MapClosed += Board_MapClosed;
		opposingUnits.AIFinishedTurnEvent += StartPlayerTurn;
		playerUnits.PlayerFinishedTurnEvent += StartAITurn;

		


	}

	private void Board_MapClosed()
	{
		foreach ( var unit in PlayerUnitsList )
		{
			unit.Destroy(); // Clean up player units	
		}
		foreach ( var unit in OpposingUnitsList )
		{
			unit.Destroy(); // Clean up opposing units	
		}
		PlayerUnitsList.Clear();
		OpposingUnitsList.Clear();
		
	}

	public void InitAStar()
	{
		AStarManager.Instance?.Initialize();
	}

	private void OnMapReady()
	{
		
		Log.Info( "Map is ready, starting gameplay systems..." );

		AStarPathFinder pathFinder = new AStarPathFinder( board );
		//TODO: Initialize gameplay systems here
		// Init Player Units - I need to find some way to load this from a config file or json.

		for ( int i = 0; i < 4; i++ )
		{
			var unit = new GameObject();
			var unitComp = unit.AddComponent<Unit>();
			var unitRender = unit.AddComponent<SkinnedModelRenderer>();
			unitRender.Model = Model.Load( "models/charactermodels/cubeguy_army.vmdl" );
			unitComp.Team = UnitTeam.Player;
			unit.Name = $"PlayerUnit_{i}";
			unitComp.SetTile( 20 + i, 15 ); // Set initial tile for the unit, this should be replaced with actual logic to set the tile based on the game state
			PlayerUnitsList.Add( unit );
		}

		int numClusters = 25;
		int zombiesPerCluster = 5;
		Random rand = new Random();

		for ( int c = 0; c < numClusters; c++ )
		{
			// Pick a random cluster center (within bounds)
			int clusterX = rand.Next( 5, board.Width - 5 );
			int clusterY = rand.Next( 5, board.Length - 5 );

			for ( int i = 0; i < zombiesPerCluster; i++ )
			{
				// Offset each zombie slightly from cluster center
				int offsetX = rand.Next( -2, 3 ); // Range: -2 to +2
				int offsetY = rand.Next( -2, 3 );

				int x = Math.Clamp( clusterX + offsetX, 0, board.Width - 1 );
				int y = Math.Clamp( clusterY + offsetY, 0, board.Length - 1 );

				var unit = new GameObject();
				var zombComp = unit.AddComponent<Zombie>();
				var unitRender = unit.AddComponent<SkinnedModelRenderer>();
				unitRender.Model = Model.Load( "models/charactermodels/cubeguy_2.vmdl" );
				unitRender.Tint = Color.Green;
				zombComp.Team = UnitTeam.AI;
				unit.Name = $"AIUnit_{c}_{i}";
				zombComp.SetTile( x, y );
				OpposingUnitsList.Add( unit );
			}
		}
		CreateWeapons();



		// Setup mission objectives
		// Initialize Player and AI units.


		// Place player units
		// Place enemy units
		// Initialize turn order



		// Start the first turn
		
		StartFirstTurn();

		// Wait for player to do turn -- Subscribe to players finished turn event.


		// Wait for enemy to do turn -- Subscribe to AI finished turn event.
		// See if game is still going
		

	}

	async void StartFirstTurn()
	{
		LevelLoadedEvent.Invoke();

		await Task.Delay( 100 ); // Wait 100ms

		StartPlayerTurn();
	}

	public void CreateWeapons()
	{
		foreach ( var unit in PlayerUnitsList )
		{
			var go = new GameObject( $"{unit.Name}_Weapon" );
			go.SetParent( unit ); // Set the weapon as a child of the unit
			go.WorldTransform = unit.WorldTransform;

			var weapon = go.AddComponent<Weapon>();
			if ( weapon == null )
			{
				Log.Warning( $"Unit {unit.Name} does not have a Weapon component." );
				continue;
			}
			// Initialize or create the weapon as needed
			weapon.ModelRenderer.Model = Model.Load( weapon.ModelPath );
			weapon.OwnerUnit = unit.GetComponent<Unit>();
			var unitComponent = unit.GetComponent<Unit>();
			unitComponent.weapon = weapon;
			Log.Info( $"Weapon {weapon.Name} created for unit {unit.Name}." );
			weapon.WorldPosition += Vector3.Up * 60f; // Raise weapon slightly above ground
			weapon.WorldPosition += Vector3.Forward * 25f; // Push Weapon forward;
			weapon.WorldPosition += Vector3.Right * 8f; // Push Weapon Right;
		}
	}
	public void StartPlayerTurn()
	{
		//Log.Info( "Player's turn started." );

		if ( ZombieCount <= 0 )
		{
			Log.Info( "All zombies defeated, player wins!" );
			WinConditionsMet = true;
			WinConditionMetEvent?.Invoke();
			return;
		}
		if ( PlayerUnitCount <= 0 )
		{
			Log.Info( "All player units defeated, AI wins!" );
			WinConditionsMet = true;
			LossConditionMetEvent?.Invoke();
			return;
		}
		StartPlayerTurnEvent?.Invoke();
		Log.Info( "StartPlayerTurnEvent invoked." );

	}

	public void StartAITurn()
	{
		//Log.Info( "AI's turn started." );
		StartAITurnEvent?.Invoke();
		
	}


}
