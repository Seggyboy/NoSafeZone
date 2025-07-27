using Sandbox;

public class Storyteller : Component
{
	[Property] public Blackboard Blackboard { get; set; }
	[Property] public EventScheduler EventScheduler { get; set; }
	[Property] public CommandProcessor CommandProcessor { get; set; }
	[Property] public StoryFilter StoryFilter { get; set; }
	//[Property] public StoryUI StoryUI { get; set}
	protected override void OnEnabled()
	{
		Blackboard = Scene.GetAllComponents<Blackboard>().FirstOrDefault();
		if ( Blackboard == null )
		{
			Log.Error( "Storyteller: Blackboard not found in the scene!" );
			return;
		}

		EventScheduler = Scene.GetAllComponents<EventScheduler>().FirstOrDefault();
		if ( EventScheduler == null )
		{
			Log.Error( "Storyteller: EventScheduler not found in the scene!" );
			return;
		}
		StoryFilter = Scene.GetAllComponents<StoryFilter>().FirstOrDefault();
		if ( StoryFilter == null )
		{
			Log.Error( "Storyteller: StoryFilter not found in the scene!" );
			return;
		}
		CommandProcessor = Scene.GetAllComponents<CommandProcessor>().FirstOrDefault();	
		TravelerEvents.ChoiceSelectedEvent += OnChoiceSelected;
	}

	protected override void OnDisabled()
	{
		TravelerEvents.ChoiceSelectedEvent -= OnChoiceSelected;
	}
	public void ExecuteEvent( StoryEvent evt )
	{
		if ( evt == null )
		{
			Log.Error( "Storyteller: Attempted to execute a null event!" );
			return;
		}
		Log.Info( $"Executing event: {evt.Name}" );
		Blackboard.FlagsSet.Add( evt.Name );

		Blackboard.IsInEvent = true;

		Log.Info( $"Executing event: {evt.Name}" );

		// Show the event UI
		TravelerEvents.RaiseStoryStartedEvent( evt );
	}


	private void OnChoiceSelected( StoryChoice choice )
	{
		Log.Info( $"Player chose: {choice.Text}" );
		var commands = choice.Consequence.Commands;
		foreach ( var cmd in commands )
		{
			CommandProcessor.Execute( cmd );
		}


		// Mark event as completed
		Blackboard.IsInEvent = false;
	}
}
