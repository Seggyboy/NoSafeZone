public static class GameConstants
{
	public static class Map
	{
		public const int LAYERHEIGHT = 120;
		public const int TILESIZE = 100;
		public const int WALLHEIGHT = 100;
	}

	public static class Combat
	{

		public const float CRITCHANCE = 0.1f;
	}

	public static class Audio
	{
		public const string FOOTSTEP = "footstep";
		public const string DEATH = "unit.death";
	}

	public static class Resources
	{

		/// <summary>
		/// FUEL RELATED CONSTANTS.
		/// </summary>
		/// 
		public const int HUMVEE_FUELCAPACITY = 25; // gallons
		public const int LMTV_FUELCAPACITY = 50; // gallons
		public const int FUELTRUCK_FUELCAPACITY = 500; // gallons
		public const float BASE_EFFICIENT_SPEED = 60f; // speed at which vehicles are most efficient fuelwise
		public const int HUMVEE_RANGE = 150; // miles
		public const int LMTV_RANGE = 250; // miles
		public const float HUMVEE_GPM = 0.14f; // Gallons per mile
		public const float LMTV_GPM = 0.14f; // Gallons per mile


		/// <summary>
		/// WEIGHT RELATED CONSTANTS
		/// </summary>
		/// 
		 
		public const int HUMVEE_CARRYINGCAPACITY = 2000; // pounds
		public const int LMTV_CARRYINGCAPACITY = 5000; // pounds
		public const int FOODWEIGHT = 1; // pounds per pound of food
		public const float AMMOWEIGHT = 0.01875f; // pounds per round of ammo
		public const int WEIGHTPERPERSON = 150; // pounds per person

		/// FOOD RELATED CONSTANTS

		public const int FOODPERPERSON = 4; // pounds per person per day
		public const int DAILYMORALELOSS = 5; // morale loss per day

		/// <summary>
		/// MORALE RELATED CONSTANTS
		/// </summary>

		public const int RESTMORALEGAIN = 10; // morale gain per rest period





	}

	public static class Travel
	{

		public const int SECONDSPERGAMEHOUR = 40; // Note: doubled travel speed.
		public const int MAXCAMERASPEED = 20;
		public const float MAXVEHICLESPEED = 80; // Maximum speed of vehicles in mph
		public const float PIXELSPERMILE = 1f; 

	}

	public static class Data
	{

		public const string REGIONFILE = "regions.json"; // File containing region data
		public const string STORYFILE = "story.json"; // File containing all story event data
		public const string TRAVELERFILE = "traveler.json"; // File containing traveler data
		public const string SAVEDATAFILE = "save_data.json"; // File for saving game state
		public const string SETTINGSFILE = "settings.json"; // File for game settings
		public const string PLAYERDATAFILE = "player_data.json"; // File for player data
		public const string MAPDATAFILE = "map_data.json"; // File for map data
	}
}
