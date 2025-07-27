using Sandbox;
using System;
using System.Diagnostics;

public static class TacticalEvents
{
	public static event Action StartPlayerTurnEvent;
	public static event Action StartAITurnEvent;
	public static event Action WinConditionMetEvent;
	public static event Action LossConditionMetEvent;
	public static event Action LevelLoadedEvent;
	public static event Action MapGeneratedEvent;
	public static event Action MapClosedEvent;
	public static event Action AIFinishedTurnEvent;
	public static event Action PlayerFinishedTurnEvent;
	public static event Action<int> ChangedLayerZEvent;

	// Helper for logging event calls
	private static void LogEvent( string name )
	{

	
		Log.Info( $"[EVENT] {name} raised" );

	}

	public static void RaiseStartPlayerTurn()
	{
		LogEvent( nameof( StartPlayerTurnEvent ) );
		StartPlayerTurnEvent?.Invoke();
	}

	public static void RaiseStartAITurn()
	{
		LogEvent( nameof( StartAITurnEvent ) );
		StartAITurnEvent?.Invoke();
	}

	public static void RaiseWinCondition()
	{
		LogEvent( nameof( WinConditionMetEvent ) );
		WinConditionMetEvent?.Invoke();
	}

	public static void RaiseLossCondition()
	{
		LogEvent( nameof( LossConditionMetEvent ) );
		LossConditionMetEvent?.Invoke();
	}

	public static void RaiseLevelLoaded()
	{
		LogEvent( nameof( LevelLoadedEvent ) );
		LevelLoadedEvent?.Invoke();
	}

	public static void RaiseMapGenerated()
	{
		LogEvent( nameof( MapGeneratedEvent ) );
		MapGeneratedEvent?.Invoke();
	}

	public static void RaiseMapClosed()
	{
		LogEvent( nameof( MapClosedEvent ) );
		MapClosedEvent?.Invoke();
	}

	public static void RaiseAIFinishedTurn()
	{
		LogEvent( nameof( AIFinishedTurnEvent ) );
		AIFinishedTurnEvent?.Invoke();
	}

	public static void RaisePlayerFinishedTurn()
	{
		LogEvent( nameof( PlayerFinishedTurnEvent ) );
		PlayerFinishedTurnEvent?.Invoke();
	}

	public static void RaiseChangedLayerZEvent(int value)
	{
		LogEvent( nameof( ChangedLayerZEvent ) );
		ChangedLayerZEvent?.Invoke( value );
	}
}
