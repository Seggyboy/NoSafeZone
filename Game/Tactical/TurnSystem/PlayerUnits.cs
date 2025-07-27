using Sandbox;
using System;

public sealed class PlayerUnits : Component
{
	
	




	protected override void OnEnabled()
	{
		TacticalEvents.StartPlayerTurnEvent += StartTurn;
	}
	protected override void OnDisabled()
	{
		TacticalEvents.StartPlayerTurnEvent -= StartTurn;
	}

	public void StartTurn()
	{

		Log.Info( "Player's turn started." );

		foreach ( var unit in BattleMaster.Instance.PlayerUnitsList )
		{
			var unitComponent = unit.GetComponent<Unit>();
			unitComponent.TimeUnits = unitComponent.MaxTimeUnits; // Reset time units for each player unit
		}



	}

	

	

	[Button( "End Turn" )]
	public void EndTurn()
	{

		TacticalEvents.RaisePlayerFinishedTurn();
	}
}
