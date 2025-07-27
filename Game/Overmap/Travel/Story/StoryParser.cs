using System;
using System.Collections.Generic;
using System.IO;
using Sandbox;

public static class StoryParser
{
	public static List<StoryEvent> ParseEvents( string filePath )
	{
		if ( !FileSystem.Data.FileExists( filePath ) )
		{
			Log.Error( $"StoryParser: File not found at {filePath}" );
			return new List<StoryEvent>();
		}

		string json = FileSystem.Data.ReadAllText( filePath );
		List<StoryEventIntermediary> dirtyEvents = Json.Deserialize<List<StoryEventIntermediary>>( json );
		List<StoryEvent> cleanEvents = new List<StoryEvent>();
		Log.Info( $"StoryParser: Parsed {dirtyEvents.Count} dirty story events from file." );

		foreach ( var dirtyEvent in dirtyEvents )
		{
			cleanEvents.Add(CleanEvent( dirtyEvent ) );
		}

		return cleanEvents;
	}

	public static StoryEvent CleanEvent(StoryEventIntermediary dirtyEvent)
	{
		var storyEvent = new StoryEvent();
		
		storyEvent.Name = dirtyEvent.Name;
		storyEvent.Trigger = dirtyEvent.Trigger;
		storyEvent.Description = dirtyEvent.Description;

		storyEvent.Choices = new List<StoryChoice>();

		storyEvent.Choices.Add(
			new StoryChoice
			{
				Text = dirtyEvent.Choice1,
				Consequence = new StoryConsequence
				{
					Text = dirtyEvent.Consequence1,
					Commands = ParseCommands( dirtyEvent.Consequence1Commands )
				}
			} );

		storyEvent.Choices.Add(
			new StoryChoice
			{
				Text = dirtyEvent.Choice2,
				Consequence = new StoryConsequence
				{
					Text = dirtyEvent.Consequence2,
					Commands = ParseCommands( dirtyEvent.Consequence2Commands )
				}
			} );
		storyEvent.Choices.Add(
			new StoryChoice
			{
				Text = dirtyEvent.Choice3,
				Consequence = new StoryConsequence
				{
					Text = dirtyEvent.Consequence3,
					Commands = ParseCommands( dirtyEvent.Consequence3Commands )
				}
			} );

		storyEvent.Tags = new List<string>();
		if ( !string.IsNullOrWhiteSpace( dirtyEvent.Tags ) )
		{
			storyEvent.Tags.AddRange( dirtyEvent.Tags.Split( ',', StringSplitOptions.RemoveEmptyEntries ) );
		}


		return storyEvent;

	}

	public static List<StoryCommand> ParseCommands( string raw )
	{
		var commands = new List<StoryCommand>();
		if ( string.IsNullOrWhiteSpace( raw ) )
			return commands;

		// Split by semicolon to get individual commands
		var parts = raw.Split( ';', StringSplitOptions.RemoveEmptyEntries );

		foreach ( var part in parts )
		{
			var trimmed = part.Trim();
			if ( string.IsNullOrWhiteSpace( trimmed ) ) continue;

			var tokens = trimmed.Split( ' ', StringSplitOptions.RemoveEmptyEntries );
			if ( tokens.Length < 2 )
			{
				Log.Warning( $"Invalid command format: '{trimmed}'" );
				continue;
			}

			var command = tokens[0].ToUpperInvariant();
			var target = tokens[1];
			var value = tokens.Length >= 3 ? tokens[2] : ""; // Some commands may not have a value

			commands.Add( new StoryCommand
			{
				Command = command,
				Target = target,
				Value = value
			} );
		}

		return commands;
	}


}
