using Sandbox;

public class Blackboard : Component
{

	// ==============================
	// System References
	// ==============================
	[Property] private VehicleRenderer VehicleTracker { get; set; }
	[Property] private WeatherDirector WeatherDirector { get; set; }
	[Property] private ResourceSimulator ResourceSimulator { get; set; }
	[Property] private ResourceTracker ResourceTracker { get; set; }
	[Property] private Traveler Traveler { get; set; }
	[Property] private TravelerMap TravelerMap { get; set; }

	[Property] private Init Init { get; set; } // Not used, but can be used to initialize the game state if needed.

	// ==============================
	// RESOURCES & INVENTORY
	// ==============================
	public int Ammo => ResourceTracker?.Ammo ?? 0;
	public int Food => ResourceTracker?.Food ?? 0;
	public float Fuel => ResourceTracker?.Fuel ?? 0f;

	public int ToolCount; //Not implemented
	public int SpareTires; //Not implemented
	public int VehicleParts;//Not implemented
	public float WeaponCondition;//Not implemented
	public float BatteryPower;//Not implemented

	// ==============================
	// CONVOY & TRAVEL STATUS
	// ==============================
	public float DistanceToDestination => Traveler?.TargetMiles ?? 0f;
	public float MilesTraveled => Traveler?.MilesTraveled ?? 0f;
	public float DistanceTraveledTotal => Traveler?.TotalMilesTraveled ?? 0f;

	public bool IsInEvent = false;
	public int TimeOfDay => Traveler?.CurrentHour ?? 0; // 0-23, where 0 is midnight and 12 is noon
	public bool IsNightTime => TimeOfDay >= 18 || TimeOfDay < 6; // 6 PM (18) to 6 AM is night
	public bool UnderAttack;//Not implemented
	public int DaysWithoutRest; //Not implemented
	public string CurrentBiome;//Not implemented
	public float Temperature;//Not implemented
	public bool IsRaining;//Not implemented
	public bool IsStorming;//Not implemented
	public float VehicleFuelEfficiency; // Not sure how to access this yet.

	// ==============================
	// PEOPLE & HEALTH
	// ==============================
	public float AvgUnitHealth;//Not implemented
	public int InjuredCount;//Not implemented
	public int DeadCount;//Not implemented
						 //public bool HasSpecialist( string role ); // e.g. "Medic", "Engineer" //Not implemented

	// ==============================
	// MORALE & PSYCHOLOGY
	// ==============================
	public int Morale => ResourceTracker?.Morale ?? 0;
	public int DespairScore; // Not implemented
	public int RuthlessnessScore; // Not implemented
	public int CharityScore; // Not implemented

	// ==============================
	// COMBAT & ENCOUNTERS
	// ==============================
	public int LastBattleOutcomeScore; // Not implemented
	public int TotalEventsTriggered; // Not implemented
	public float ZombieDensityNearby; // Not implemented

	// ==============================
	// STORY & TRIGGER SYSTEM
	// ==============================
	public HashSet<string> FlagsSet;
	[Property] public List<StoryEvent> AllEvents => Init?.StoryEvents;
	//public bool HasItem( string itemName ); // e.g. "Geiger Counter" // not implemented



	protected override void OnUpdate()
	{
		if ( ResourceTracker == null )
		{
			ResourceTracker = Scene.GetAllComponents<ResourceTracker>().FirstOrDefault();
		}
		if ( ResourceSimulator == null )
		{
			ResourceSimulator = Scene.GetAllComponents<ResourceSimulator>().FirstOrDefault();
		}
		if ( Init == null )
		{
			Init = Scene.GetAllComponents<Init>().FirstOrDefault();
		}
	}


	protected override void OnEnabled()
	{
		Initialize();
	}


	public void Initialize()
	{
		VehicleTracker = Scene.GetAllComponents<VehicleRenderer>().FirstOrDefault();
		if ( VehicleTracker == null )
		{
			Log.Error( "Blackboard: VehicleTracker not found in the scene!" );
			return;
		}
		WeatherDirector = Scene.GetAllComponents<WeatherDirector>().FirstOrDefault();
		if ( WeatherDirector == null )
		{
			Log.Error( "Blackboard: WeatherDirector not found in the scene!" );
			return;
		}
		ResourceSimulator = Scene.GetAllComponents<ResourceSimulator>().FirstOrDefault();
		if ( ResourceSimulator == null )
		{
			Log.Error( "Blackboard: ResourceSimulator not found in the scene!" );
			return;
		}
		ResourceTracker = Scene.GetAllComponents<ResourceTracker>().FirstOrDefault();
		if ( ResourceTracker == null )
		{
			Log.Error( "Blackboard: ResourceTracker not found in the scene!" );
			return;
		}
		Traveler = Scene.GetAllComponents<Traveler>().FirstOrDefault();
		if ( Traveler == null )
		{
			Log.Error( "Blackboard: Traveler not found in the scene!" );
			return;
		}
	}

	



}
