using System;
using Sandbox;
using static Sandbox.Citizen.CitizenAnimationHelper;

public sealed class StoryFilter : Component
{
	[Property] public Blackboard Blackboard { get; set; }
	[Property] public List<StoryEvent> AllEvents => Blackboard?.AllEvents;

	public Random Rand = new Random();
	public List<StoryEvent> FilteredEvents { get; private set; } = new();

	protected override void OnUpdate()
	{
		if ( Blackboard == null || AllEvents == null )
			return;

	
	}

	private bool IsEventValid( StoryEvent evt )
	{
		// Example future conditions (once implemented)
		// Replace magic numbers with game constants!

		if ( evt.Tags.Contains( "NightOnly" ) && !Blackboard.IsNightTime )
			return false;

		if ( evt.Tags.Contains( "NeedsHighMorale" ) && Blackboard.Morale < 50 )
			return false;

		// Placeholder — always allow until conditions exist
		return true;
	}


	public StoryEvent PickValidRandomEvent()
	{
		const int MaxAttempts = 10;

		for ( int i = 0; i < MaxAttempts; i++ )
		{
			var candidate = AllEvents[Rand.Int( 0, AllEvents.Count - 1 )];
			if ( IsEventValid( candidate ) )
				return candidate;
		}
		return null;
	}

}
