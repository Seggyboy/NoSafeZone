// LINQ Cheat Sheet for C# - Game Dev Friendly
/*using System;
using System.Collections.Generic;
using System.Linq;

public class Unit
{
	public string Name;
	public int Health;
	public int Morale;
	public string Role;
	public bool IsInjured;
	public bool IsAlive => Health > 0;
}

public class LINQExamples
{
	public void Run()
	{
		List<Unit> units = new List<Unit>
		{
			new Unit { Name = "Smith", Health = 100, Morale = 80, Role = "Rifleman", IsInjured = false },
			new Unit { Name = "Jones", Health = 45, Morale = 60, Role = "Medic", IsInjured = true },
			new Unit { Name = "Lee", Health = 0, Morale = 30, Role = "Sniper", IsInjured = true },
			new Unit { Name = "Davis", Health = 75, Morale = 90, Role = "Engineer", IsInjured = false }
		};

		// =======================
		// FILTERING
		// =======================

		// Get all alive units
		var aliveUnits = units.Where( u => u.IsAlive ).ToList();

		// Get all injured medics
		var injuredMedics = units.Where( u => u.Role == "Medic" && u.IsInjured ).ToList();

		// =======================
		// SELECTING
		// =======================

		// Get all unit names
		var names = units.Select( u => u.Name ).ToList();

		// Get morale values only
		var moraleValues = units.Select( u => u.Morale ).ToList();

		// =======================
		// ORDERING
		// =======================

		// Sort by morale descending
		var orderedByMorale = units.OrderByDescending( u => u.Morale ).ToList();

		// Sort by health ascending
		var orderedByHealth = units.OrderBy( u => u.Health ).ToList();

		// =======================
		// FIRST / SINGLE
		// =======================

		// First alive unit
		var firstAlive = units.FirstOrDefault( u => u.IsAlive ); // Returns null if none

		// Single sniper (throws if more than one)
		var sniper = units.SingleOrDefault( u => u.Role == "Sniper" );

		// =======================
		// ANY / ALL
		// =======================

		// Are any units dead?
		bool anyDead = units.Any( u => !u.IsAlive );

		// Are all units above 50 morale?
		bool allHighMorale = units.All( u => u.Morale > 50 );

		// =======================
		// GROUPING
		// =======================

		// Group by role
		var groupedByRole = units.GroupBy( u => u.Role );
		foreach ( var group in groupedByRole )
		{
			Console.WriteLine( $"Role: {group.Key}" );
			foreach ( var unit in group )
				Console.WriteLine( " - " + unit.Name );
		}

		// =======================
		// COUNT / SUM / AVERAGE
		// =======================

		int totalUnits = units.Count();
		int injuredCount = units.Count( u => u.IsInjured );
		int totalHealth = units.Sum( u => u.Health );
		double avgMorale = units.Average( u => u.Morale );

		// =======================
		// DISTINCT
		// =======================

		var distinctRoles = units.Select( u => u.Role ).Distinct().ToList();
	}
}*/
