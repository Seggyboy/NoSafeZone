public class StoryEventIntermediary
{
	public string Name { get; set; }
	public string Trigger { get; set; }
	public string Description { get; set; }

	public string Choice1 { get; set; }
	public string Choice2 { get; set; }
	public string Choice3 { get; set; }

	public string Consequence1 { get; set; }
	public string Consequence1Commands { get; set; }

	public string Consequence2 { get; set; }
	public string Consequence2Commands { get; set; }

	public string Consequence3 { get; set; }
	public string Consequence3Commands { get; set; }

	public string Tags { get; set; }
}

public class StoryEvent
{
	public string Name { get; set; }
	public string Trigger { get; set; }
	public string Description { get; set; }
	public List<StoryChoice> Choices { get; set; } = new List<StoryChoice>();
	public List<string> Tags { get; set; } = new List<string>(); // e.g., "NightOnly", "NeedsHighMorale"


}

public class StoryChoice
{
	public string Text;
	public StoryConsequence Consequence;
}

public class StoryConsequence
{
	public string Text; // e.g., "You found some food!"
	public List<StoryCommand> Commands; // e.g., modify resources, trigger events
}



public class StoryCommand
{
	public string Command;   // e.g., "MODIFY"
	public string Target;    // e.g., "FOOD"
	public string Value;     // e.g., "-10"
}

