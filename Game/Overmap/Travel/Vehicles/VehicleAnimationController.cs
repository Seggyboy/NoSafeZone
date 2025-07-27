using Sandbox;

public sealed class VehicleAnimationController : Component
{
	[Property] VehicleRenderer VehicleRenderer;
	[Property] public List<GameObject> Vehicles => VehicleRenderer?.Vehicles ?? null;

	protected override void OnEnabled()
	{
		TravelerEvents.ConvoyMovingEvent += StartRumbleAll;
		TravelerEvents.ConvoyStopEvent += StopRumbleAll;
		StopRumbleAll();
	}

	protected override void OnDisabled()
	{
		TravelerEvents.ConvoyMovingEvent -= StartRumbleAll;
		TravelerEvents.ConvoyStopEvent -= StopRumbleAll;
	}
	public void StopRumble( string vehicleName )
	{
		var vehicle = Vehicles.Find( v => v.Name == vehicleName );
		if ( vehicle == null )
		{
			Log.Error( $"Vehicle '{vehicleName}' not found." );
			return;
		}
		SetRumble( vehicle, bumpMagnitude: 0f, bumpSpeed: 0f, xBump: 0f, yBump: 0f, zBump: 0f );
	}
	[Button( "Stop Rumble All" )]
	public void StopRumbleAll()
	{
		SetRumbleAll( 0f, 0f, 0f, 0f, 0f );
	}

	public void StartRumble( string vehicleName )
	{
		var vehicle = Vehicles.Find( v => v.Name == vehicleName );
		if ( vehicle == null )
		{
			Log.Error( $"Vehicle '{vehicleName}' not found." );
			return;
		}
		SetRumble( vehicle );
	}

	[Button( "Start Rumble All" )]
	public void StartRumbleAll()
	{
		SetRumbleAll();
	}

	public void SetRumbleAll(
	float? bumpMagnitude = null, float? bumpSpeed = null,
	float? xBump = null, float? yBump = null, float? zBump = null )
	{
		float magnitude = bumpMagnitude ?? 1f;
		float speed = bumpSpeed ?? 10f;
		float x = xBump ?? 0.03f;
		float y = yBump ?? 0.03f;
		float z = zBump ?? 0.03f;

		foreach ( var vehicle in Vehicles )
		{
			var rumble = vehicle.Components.Get<RumbleEffect>();
			if ( rumble == null ) continue;

			rumble.BumpMagnitude = magnitude;
			rumble.BumpSpeed = speed;
			rumble.XBumpMultiplier = x;
			rumble.YBumpMultiplier = y;
			rumble.ZBumpMultiplier = z;
		}
	}


	public void SetRumble(
		GameObject vehicle,
		float? bumpMagnitude = null, float? bumpSpeed = null,
		float? xBump = null, float? yBump = null, float? zBump = null )
	{
		var rumble = vehicle.Components.Get<RumbleEffect>();
		if ( rumble == null ) return;

		if ( bumpMagnitude.HasValue ) rumble.BumpMagnitude = bumpMagnitude.Value;
		if ( bumpSpeed.HasValue ) rumble.BumpSpeed = bumpSpeed.Value;
		if ( xBump.HasValue ) rumble.XBumpMultiplier = xBump.Value;
		if ( yBump.HasValue ) rumble.YBumpMultiplier = yBump.Value;
		if ( zBump.HasValue ) rumble.ZBumpMultiplier = zBump.Value;
	}
}
