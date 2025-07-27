using Sandbox;


public class Init : Component
{
	[Property] public List<StoryEvent> StoryEvents { get; set; }
	// This class is used to intialize static classes that aren't components.

	protected override void	OnEnabled()
	{
		Initialize();
	}
	public void Initialize()
	{
		
		StoryEvents = StoryParser.ParseEvents(GameConstants.Data.STORYFILE );
	}
}
