using System;
using Sandbox;

public sealed class ResourceTracker : Component
{
	[Property] public int Ammo { get; private set; } // individual rounds
	[Property] public float Fuel { get; private set; } // Gallons of fuel
	[Property] public int Food { get; private set; } // Pounds of food
	[Property] public int Morale { get; private set; } // 0-100
	[Property] public int Soldiers { get; private set; } = 32; // This is not permanent. We'll hook into the unit manager list later.
	protected override void OnUpdate()
	{

	}

	public void ModAmmo( int amount )
	{
		Ammo += amount;
		TravelerEvents.RaiseAmmoChanged();
	}

	public void ModFuel( float amount )
	{
		Fuel += amount;
		TravelerEvents.RaiseFuelChanged();
	}
	public void ModFood( int amount )
	{
		Food += amount;
		TravelerEvents.RaiseFoodChanged();
	}
	public void ModMorale( int amount )
	{
		Morale = Math.Clamp( Morale + amount, 0, 100 );
		TravelerEvents.RaiseMoraleChanged();
	}


	public void SetAmmo( int amount )
	{
		Ammo = amount;
		TravelerEvents.RaiseAmmoChanged();
	}

	public void SetFuel( float amount )
	{
		Fuel = amount;
		TravelerEvents.RaiseFuelChanged();
	}
	public void SetFood( int amount )
	{
		Food = amount;
		TravelerEvents.RaiseFoodChanged();
	}
	public void SetMorale( int amount )
	{
		Morale = Math.Clamp( amount, 0, 100 );
		TravelerEvents.RaiseMoraleChanged();
	}

	public void ResetResources()
	{
		Ammo = 0;
		Fuel = 0;
		Food = 0;
		Morale = 100;
	}
}


/*
 * 	          GPM	FUEL TANK SIZE (GAL)	RANGE
HUMVEE	0.1428571429	25	               150Mi
LMTV	0.1428571429	50	               250Mi
FUEL TRUCK	N/A	500	N/A
 * 
 * 
VEHICLE CAPACITY (IB)
HUMVEE		2000
LMTV		5000
FUEL TRUCK		500 (GALLONS)
 * 
 * Combat Loads (Squad)			
Rounds	Grenades	Smoke
1SL	210	2	2
2RFL	210	2	2
2AR	800	1	2
2GL	210	12 (GL)	2
2TL	210	2	2
Squad Combat Load	1640	7	10
Platoon Combat Load	6560		
 * 
 * 
Starting Vehicles	Tank Capacity
Humvee	2	50
LMTV	2	100
 Fuel Truck	1	500
Convoy AVG GPM	0.5714285714	
 * 
 * 
 Starting Resources	Days/SquadCombatLoads/Miles	Weight
			
Food (IBs)	780	5.416666667	780
Ammo (Rounds)	10000	6.097560976	187.5
Fuel (Gallons)	650	1137.5	N/A
Morale (0-100)	85		
Units	32		4800
	FOOD CONSUMPTION/Day		
Per Person Food	4.5		
Soldiers	35		
Total	157.5		
 * 
 * 
 * 
 * 
 * 
 * 
 * TotalRange = (TotalGallonsAvailable) / (AvgConvoyGPM)
DaysOfFood = (TotalFoodLbs) / (4.5 * NumSoldiers)
Extra Fuel Penalty = (Current Load / Max Load) ^ 2
 **/
