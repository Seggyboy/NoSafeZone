using Sandbox;


public sealed class CommandProcessor : Component
{
	[Property] public Blackboard Blackboard { get; set; }

	[Property] public ResourceTracker ResourceTracker { get; set; }

	protected override void OnUpdate()
	{
		if (ResourceTracker == null )
		{
			ResourceTracker = Scene.GetAllComponents<ResourceTracker>().FirstOrDefault();
		}
	}
	public void Execute(StoryCommand cmd)
	{
		var cmdName = cmd.Command.ToUpperInvariant();
		var value = cmd.Value;
		var target = cmd.Target.ToUpperInvariant();
		switch ( cmdName )
		{
			case "MODIFY":
					Modify(target, value.ToFloat() );
				break;
			case "SET":
				Log.Info( "Not implemented: SET command" );
				break;
			case "START_BATTLE":
				Log.Info( "Not implemented: START_BATTLE command" );
				break;
			case "TRIGGER":
				Log.Info( "Not implemented: TRIGGER command" );
				break;
			case "SETSPECIAL":
				Log.Info( "Not implemented: SETSPECIAL command" );
				break;
			case "HURT":
				Log.Info( "Not implemented: HURT command" );
				break;
			case "CREATELOCATION":
				Log.Info( "Not implemented: CREATELOCATION command" );
				break;
			case "SCHEDULEEVENT":
				Log.Info( "Not implemented: SCHEDULEEVENT command" );
				break;


		}

	}

	public void Modify(string target, float value)
	{
		if ( Blackboard == null )
		{
			Log.Error( "CommandProcessor: Blackboard not found in the scene!" );
			return;
		}
		switch ( target )
		{
			case "FOOD":
				ResourceTracker.ModFood( (int) value );
				break;
			case "AMMO":
				ResourceTracker.ModAmmo( (int)value );
				break;
			case "FUEL":
				ResourceTracker.ModFuel( (float)value );
				break;
			case "MORALE":
				ResourceTracker.ModMorale( (int)value );
				break;
			default:
				Log.Warning( $"Unknown target for Modify command: {target}" );
				break;
		}
	}
}


/*MODIFY
SET
START_BATTLE We can't init battles yet because we dont have persistence between scenes yet. We'll start writing data to json soon.
TRIGGER 
SETSPECIAL
HURT 
CREATELOCATION
SCHEDULEEVENT*/
