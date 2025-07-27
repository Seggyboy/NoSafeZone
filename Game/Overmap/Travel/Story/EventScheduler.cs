using System;
using Sandbox;


public class EventScheduler : Component
{
	[Property] public Blackboard Blackboard { get; set; }
	[Property] public StoryFilter StoryFilter { get; set; }
	[Property] public float BaseEventInterval = 60f; // Time in seconds between events
	[Property] private float EventInterval; // the actual next interval, modified each time
	public float IntervalJitter = 20f; // +/- seconds Maximum time between events is 80s, min is 60s.
	[Property] private float timeSinceLastEvent = 0f;
	private Random rand = new Random();

	protected override void OnEnabled()
	{
		Blackboard = Scene.GetAllComponents<Blackboard>().FirstOrDefault();
		if ( Blackboard == null )
		{
			Log.Error( "EventScheduler: Blackboard not found in the scene!" );
			return;
		}
		StoryFilter = Scene.GetAllComponents<StoryFilter>().FirstOrDefault();
		if ( StoryFilter == null )
		{
			Log.Error( "EventScheduler: StoryFilter not found in the scene!" );
			return;
		}
		EventInterval = GetNextInterval(); // Initialize the first event interval
	}
	protected override void OnUpdate()
	{
		if ( Blackboard == null || StoryFilter == null )
			return;
		if ( Blackboard.IsInEvent )
			return; // Don't trigger or schedule events while already in one
		timeSinceLastEvent += Time.Delta;
		if ( timeSinceLastEvent >= EventInterval )
		{
			timeSinceLastEvent = 0f;
			TriggerRandomEvent();
		}
	}
	private void TriggerRandomEvent()
	{
		var evt = StoryFilter.PickValidRandomEvent();
		if ( evt != null )
		{
			Log.Info( $"Triggering event: {evt.Name}" );
			EventInterval = GetNextInterval();
			//Storyteller.ExecuteEvent( evt) ;
			// Here you would handle the event logic, e.g. updating the game state, notifying players, etc.
		}
		else
		{
			Log.Info( "No valid events to trigger at this time." );
		}
	}

	


	private float GetNextInterval()
	{
		return BaseEventInterval + rand.Float( -IntervalJitter, IntervalJitter );
	}
}
