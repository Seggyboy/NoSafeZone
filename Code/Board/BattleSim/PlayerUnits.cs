using Sandbox;
using System;

public sealed class PlayerUnits : Component
{
	[Property] private BattleMaster battleMaster;
	public delegate void FinishedTurnEventHandler();
	public event FinishedTurnEventHandler PlayerFinishedTurnEvent;




	protected override void OnEnabled()
	{
		battleMaster = Scene.GetAllComponents<BattleMaster>().FirstOrDefault();
		battleMaster.StartPlayerTurnEvent += StartTurn;
		


	}

	public void StartTurn()
	{

		Log.Info( "Player's turn started." );

		foreach ( var unit in battleMaster.PlayerUnitsList )
		{
			var unitComponent = unit.GetComponent<Unit>();
			unitComponent.TimeUnits = unitComponent.MaxTimeUnits; // Reset time units for each player unit
		}



	}

	

	

	[Button( "End Turn" )]
	public void EndTurn()
	{
		PlayerFinishedTurnEvent?.Invoke();
	}
}
