using Sandbox;
using System;
using System.Threading.Tasks;

public sealed class OpposingUnits : Component
{
	[Property] private BattleMaster battleMaster;

	
	[Property] public List<Unit> KnownEnemy { get; set; }


	public delegate void FinishedTurnEventHandler();
	public event FinishedTurnEventHandler AIFinishedTurnEvent;
	protected override void OnEnabled()
	{
		battleMaster = Scene.GetAllComponents<BattleMaster>().FirstOrDefault();
		battleMaster.StartAITurnEvent += StartTurn;
	
		KnownEnemy = new List<Unit>();
	}

	public async void StartTurn()
	{
		
		Log.Info( "AI's turn started." );

		foreach ( var unit in battleMaster.OpposingUnitsList )
		{
			var unitComponent = unit.GetComponent<Unit>();
			unitComponent.TimeUnits = 40;
		}


		Look();
		// Have individual enemies attack if able to
		// If not possible, Move individual enemies to their closest known enemy unit if one exists.


		//Do AI Behavior
		await HandleAITurn();
		



	}

	private async Task HandleAITurn()
	{
		foreach ( var go in battleMaster.OpposingUnitsList.ToList() )
		{
			if ( go == null ) continue;
			var zombie = go.Components.Get<Zombie>();
			if ( zombie == null || zombie.unitState != UnitState.Idle ) continue;

			zombie.TimeUnits = zombie.MaxTimeUnits;
			zombie.Done = false;
			zombie.pathed = false;
			await zombie.PlanAndMove();
			
			

			
			await Task.Delay( 50 );
		}

		// End AI turn
		DelayEndTurn();
	}

	async void DelayEndTurn()
	{
		Log.Info( "AI is waiting 100ms before ending..." );
		await Task.Delay( 100 ); // Wait 100ms
		EndTurn();
	}

	public void Look()
	{
		Log.Info( "AI is detecting enemies." );
		foreach (var unit in battleMaster.OpposingUnitsList)
		{
			List<Tile> tiles = AStarManager.Instance?.RequestTilesInRange( unit.GetComponent<Unit>().CurrentTile, 30);
			foreach ( var tile in tiles)
			{
				if ( tile.unit != null && tile.unit.Team != unit.GetComponent<Unit>().Team )
				{
					Log.Info( $"AI detected enemy unit: {tile.unit.GameObject.Name} at tile {tile.WorldPosition}" );
					if ( !KnownEnemy.Contains( tile.unit ) )
					{
						KnownEnemy.Add( tile.unit );
					}
				}
			}


		}
		
	}
	[Button( "End Turn" )]
	public void EndTurn()
	{
		Log.Info( "AI is done." );
		AIFinishedTurnEvent?.Invoke();
	}

	public void RecieveOpposingUnits( List<GameObject> units )
	{
		

		battleMaster.OpposingUnitsList = units;
		Log.Info( $"Received {units.Count} opposing units." );
		for ( int i = 0; i < battleMaster.OpposingUnitsList.Count; i++ )
		{
			var go = battleMaster.OpposingUnitsList[i];
			Log.Info( $"Unit: {go.Name}" );
			var unit = go.GetComponent<Unit>();
			unit.SetTile( 0 + i, 1 ); // Set initial tile for the unit, this should be replaced with actual logic to set the tile based on the game state
			var rend = go.GetComponent<SkinnedModelRenderer>();
			if ( rend != null )
			{
				rend.Tint = Color.Green;
			}
		}
	}
}
