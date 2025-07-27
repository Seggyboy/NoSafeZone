using Sandbox;
using System;
using System.Collections.Generic;


public static class TravelerEvents
{
	public static event Action FoodChangedEvent;
	public static event Action FuelChangedEvent;
	public static event Action AmmoChangedEvent;
	public static event Action MoraleChangedEvent;
	public static event Action ConvoyMovingEvent;
	public static event Action ConvoyStopEvent;
	public static event Action ReachedDestinationEvent;
	public static event Action StartMoveConvoyEvent; // this is use to start the process of the convoy moving versus the actual movement
	public static event Action StopMoveConvoyEvent; // this is use to stop the process of the convoy moving versus the actual movement
	public static event Action OpenMapEvent; 
	public static event Action CloseMapEvent;
	public static event Action PatrolBaseEvent; // used when the convoy specifically stops to rest, instead of just stopping.


	//========== BASIC EVENTS ==========//



	// All events invoked with public raisers.

	private static void LogEvent( string name )
	{


		Log.Info( $"[EVENT] {name} raised" );

	}

	public static void RaiseFoodChanged()
	{
		FoodChangedEvent?.Invoke();
		LogEvent( nameof( FoodChangedEvent ) );
	}

	public static void RaiseFuelChanged()
	{
		FuelChangedEvent?.Invoke();
		LogEvent( nameof( FuelChangedEvent ) );
	}
	public static void RaiseAmmoChanged()
	{
		AmmoChangedEvent?.Invoke();
		LogEvent( nameof( AmmoChangedEvent ) );
	}
	public static void RaiseMoraleChanged()
	{
		MoraleChangedEvent?.Invoke();
		LogEvent( nameof( MoraleChangedEvent ) );
	}

	public static void RaiseConvoyMoving()
	{
		ConvoyMovingEvent?.Invoke();
		LogEvent( nameof(ConvoyMovingEvent) );
	}

	public static void RaiseConvoyStop()
	{
		ConvoyStopEvent?.Invoke();
		LogEvent( nameof( ConvoyStopEvent ) );
	}

	public static void RaiseDestinationReachedEvent()
	{
		ReachedDestinationEvent?.Invoke();
		LogEvent( nameof( ReachedDestinationEvent ) );
	}

	public static void RaiseStartMoveConvoyEvent()
	{
		StartMoveConvoyEvent?.Invoke();
		LogEvent( nameof( StartMoveConvoyEvent ) );
	}
	public static void RaiseStopMoveConvoyEvent()
	{
		StopMoveConvoyEvent?.Invoke();
		LogEvent( nameof( StopMoveConvoyEvent ) );
	}
	public static void RaiseOpenMapEvent()
	{
		OpenMapEvent?.Invoke();
		LogEvent( nameof( OpenMapEvent ) );
	}
	public static void RaiseCloseMapEvent()
	{
		CloseMapEvent?.Invoke();
		LogEvent( nameof( CloseMapEvent ) );
	}
	public static void RaisePatrolBaseEvent()
	{
		PatrolBaseEvent?.Invoke();
		LogEvent( nameof( PatrolBaseEvent ) );
	}


	//========== STORYTELLER EVENTS ==========//

	public static event Action<StoryEvent> StoryStartedEvent; // Raised when the storyteller starts an event, e.g. a story event or a random encounter.
	public static event Action<StoryChoice> ChoiceSelectedEvent; 

	// All events invoked with public raisers.
	public static void RaiseStoryStartedEvent(StoryEvent story)
	{
		StoryStartedEvent?.Invoke( story );
		LogEvent( nameof( StoryStartedEvent ) );
	}
	public static void RaiseChoiceSelectedEvent(StoryChoice choice)
	{
		ChoiceSelectedEvent?.Invoke(choice);
		LogEvent( nameof( ChoiceSelectedEvent ) );
	}

}

