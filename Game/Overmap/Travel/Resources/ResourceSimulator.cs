
using System;
using Sandbox;

public sealed class ResourceSimulator : Component
{
	[Property] ResourceTracker Tracker;
	// this will not track anything to do with time. It just passes hours and drains fuel.
	protected override void OnEnabled()
	{
		Initialize();
	}

	public void Initialize()
	{
		Tracker.SetFood( 780 );
		Tracker.SetFuel( 100 ); // Set initial fuel to full tank
		Tracker.SetAmmo( 10000 ); // Set initial ammo
		Tracker.SetMorale( 100 ); // Set initial morale to 100
	}

	public void PassHour(float milesTraveled, float speedMPH)
	{
		if ( Tracker == null )
		{
			Log.Error( "ResourceSimulator: Tracker is not set!" );
			return;
		}
		// Simulate resource consumption
		var foodConsumed = ConsumeFood();
		var moraleConsumed = ConsumeMorale();


		Log.Info( $"Hour passed: Food consumed: {foodConsumed}, Morale consumed: {moraleConsumed}");
	}


	public float ConsumeFuel( float milesTraveled, float speedMph )
	{
		if ( Tracker.Fuel <= 0 || milesTraveled <= 0 )
			return 0f;


		// Scale fuel consumption linearly (you could use a curve for realism)
		float speedFactor = speedMph / GameConstants.Resources.BASE_EFFICIENT_SPEED;

		// Clamp to a minimum of 0.5 and max of 2.0 to avoid extreme values
		speedFactor = Math.Clamp( speedFactor, 0.5f, 2.0f );

		float adjustedGPM = GameConstants.Resources.HUMVEE_GPM * speedFactor;
		float fuelConsumed = milesTraveled * adjustedGPM;

		Tracker.ModFuel( -fuelConsumed );
		return fuelConsumed;
	}

	public float ConsumeFood()
	{
		int foodConsumed = 0;
		if ( Tracker.Food > 0 )
		{
			
			foodConsumed = (GameConstants.Resources.FOODPERPERSON * Tracker.Soldiers) / 24;
			Tracker.ModFood( -foodConsumed );
		}
		return foodConsumed;
	}

	public float ConsumeMorale()
	{
		int moraleConsumed = 0;


		if ( Tracker.Morale > 0 )
		{
			moraleConsumed = GameConstants.Resources.DAILYMORALELOSS / 24;
			Tracker.ModMorale( -moraleConsumed );
		}
		return moraleConsumed;
	}
}
