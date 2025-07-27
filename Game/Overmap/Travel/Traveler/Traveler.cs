using System;
using Sandbox;
using SpriteTools;

public sealed class Traveler : Component
{
	[Property] CameraComponent Camera;
	[Property] ResourceSimulator ResourceSimulator;
	[Property] ResourceTracker ResourceTracker;
	[Property] VehicleRenderer VehicleRenderer;
	[Property] float SpeedMph = 70;
	[Property] bool Moving = false;

	[Property] public float MilesTraveled = 0;
	[Property] private float SimulatedHourProgress = 0f;
	[Property] private float MilesThisHour = 0f;
	[Property] public float TotalMilesTraveled = 0f; // Total miles traveled in the game session
	[Property] public int CurrentHour = 6; // time 0-23
	[Property] public int TargetMiles = 1000;
	[Property] public int ConvoySpeed = 0;


	protected override void OnEnabled()
	{
		
	}

	protected override void OnDisabled()
	{
		
	}
	protected override void OnUpdate()
	{
		if (  Camera == null )
			return;
		if ( ResourceSimulator == null )
		{
			BootTraveler(); return;

		}


		SimulateGame();

		MoveCamera();
	

		
	}


	public void BootTraveler()
	{
		ResourceSimulator = Scene.GetAllComponents<ResourceSimulator>().FirstOrDefault();
		if ( ResourceSimulator == null )
		{
			Log.Error( "Traveler: ResourceSimulator not found in the scene!" );

		}
		ResourceTracker = Scene.GetAllComponents<ResourceTracker>().FirstOrDefault();
		if ( ResourceTracker == null )
		{
			Log.Error( "Traveler: ResourceTracker not found in the scene!" );
		}

	}



	public void RestHours( int hours )
	{
		if ( !Moving )
		{
			for ( int x = 0; x < hours; x++ )
			{
				OnHourPassed();
			}
		}

	}

	public void MoveCamera()
	{

		if ( Camera.WorldPosition.y > 10000 )
		{
			Camera.WorldPosition = new Vector3( 0, 0, 0 ); // Reset position
		}
		var scalar =  Math.Round((float) (SpeedMph / GameConstants.Travel.MAXVEHICLESPEED));
		var deltaCameraY = (float) (GameConstants.Travel.MAXCAMERASPEED * Time.Delta * scalar) ;
		Camera.WorldPosition += new Vector3( 0, deltaCameraY, 0 );
	}


	public void SimulateGame()
	{

		SpeedMph = Math.Clamp( SpeedMph, 1f, GameConstants.Travel.MAXVEHICLESPEED );
		float gameHoursPassed = Time.Delta / GameConstants.Travel.SECONDSPERGAMEHOUR;

		if ( Moving )
		{
			float deltaMiles = SpeedMph * gameHoursPassed;
			ResourceSimulator.ConsumeFuel( deltaMiles, SpeedMph );
			MilesTraveled += deltaMiles;
			MilesThisHour += deltaMiles;
			TotalMilesTraveled += deltaMiles;

			if ( MilesTraveled >= TargetMiles )
			{
				DestinationReached();
			}

		}

		SimulatedHourProgress += gameHoursPassed;

		if ( SimulatedHourProgress >= 1f )
		{
			OnHourPassed();
			SimulatedHourProgress -= 1f; // retain fractional hours (e.g. 1.4 -> 0.4)
		}
		



	}

	private void OnHourPassed()
	{
		
		CurrentHour = (CurrentHour + 1) % 24; // Increment hour and wrap around at 24
		Log.Info( $"1 hour passed. Traveled: {MilesThisHour} miles. It is {CurrentHour}00. " );
		if ( ResourceSimulator != null )
		{
			ResourceSimulator.PassHour( MilesThisHour, SpeedMph );
			MilesThisHour = 0f;
		}
		else
		{
			Log.Error( "ResourceSimulator is not set!" );
		}
	}

	public void Travel(int destMiles, int startingSpeed)
	{
		TargetMiles = destMiles;
		ConvoySpeed = startingSpeed;
		MilesThisHour = 0f;
		MilesTraveled = 0f;
		StartMoving();

	}

	[Button( "Start Moving" )]
	public void StartMoving()
	{
		
		if ( !Moving )
		{
			Moving = true;
			SpeedMph = ConvoySpeed;
			foreach (var vehicle in VehicleRenderer.Vehicles )
			{
				if ( vehicle == null )
					continue;
				var spriteRenderer = vehicle.Components.Get<SpriteComponent>();
				spriteRenderer.PlayAnimation( "rolling", true );
			}
			TravelerEvents.RaiseConvoyMoving();
		}
		else
		{
			Log.Warning( "Traveler is already moving!" );
		}
	}

	[Button( "Stop Moving" )]
	public void StopMoving()
	{
		if ( Moving )
		{
			Moving = false;
			SpeedMph = 0;
			Log.Info( "Traveler stopped moving." );

			foreach ( var vehicle in VehicleRenderer.Vehicles )
			{
				if ( vehicle == null )
					continue;
				var spriteRenderer = vehicle.Components.Get<SpriteComponent>();
				spriteRenderer.PlayAnimation( "idle", true );
			}
			TravelerEvents.RaiseConvoyStop();
		}
	
	}

	public void DestinationReached()
	{
		StopMoving();
		TravelerEvents.RaiseDestinationReachedEvent();
		MilesTraveled = 0f; // Reset traveled distance

	}

	public void SetDistance( int miles )
	{
		if ( miles < 0 )
		{
			Log.Error( "Distance cannot be negative!" );
			return;
		}
		TargetMiles = miles;
		MilesTraveled = 0f; // Reset traveled distance
		MilesThisHour = 0f; // Reset this hour's distance
		Log.Info( $"Traveler distance set to {TargetMiles} miles." );
	}




}
