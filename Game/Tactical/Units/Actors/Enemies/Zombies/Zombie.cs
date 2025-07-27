using Sandbox;
using System.Threading.Tasks;

public sealed class Zombie : Unit
{
	[Property] public bool pathed = false;
	[Property] private float AttackTimer = 0.5f;
	[Property] private float AttackTimerElapsed = 0f;
	[Property] public bool reachedDest;
	[Property] public bool Done = true;

	protected override void OnEnabled()
	{
		base.OnEnabled();

		MaxTimeUnits = 15;
	}
	protected override void OnUpdate()
	{
		if ( TimeUnits <= 3 || Done == true)
		{
			Done = true;
			return;
		}
		
			
		if ( Path == null )
		{
			//Log.Info( $"{GameObject.Name} has reached its destination tile." );
			reachedDest = true;
			
		}


		if (reachedDest && AttackTimerElapsed > AttackTimer && !Done )
		{
			AttackTimerElapsed = 0f;
			AttemptAttack();
			
		}


		AttackTimerElapsed += Time.Delta;





		base.OnUpdate();

		

		
	}

	

	public async Task PlanAndMove()
	{
		if ( Done ) return; // If already done, no need to path again
		var enemies = FindEnemiesInRange( VisionRadius );
		if ( enemies.Count > 0 )
		{
			var closest = GetClosestUnit( enemies );
			var adjacentTile = AStarManager.Instance?.GetAdjacentFreeTile( closest.CurrentTile, this.CurrentTile );
			if ( adjacentTile != null )
			{
				MoveTo( adjacentTile );
				reachedDest = false; // Reset reachedDest since we're moving
				pathed = true;
			}
			else
			{
				//Log.Info( "No adjacent tile found to move to!" );
				Done = true;
			}
		}
		else
		{
			//Log.Info( "No enemies in range for " + GameObject.Name );
			Done = true;
		}
	}

	public void AttemptAttack()
	{
		Log.Info( "ATTEMPTING ATTACK" );
		var closest = GetClosestUnit( FindEnemiesInRange( 1 ) );
		if ( closest == null )
		{
			//Log.Info( "No enemies found to attack!" );
			Done = true;
			return;
		}
		if ( this.CurrentTile == null || closest.CurrentTile == null )
		{
			Log.Warning( "Null tile in attack check." );
			Done = true;
			return;
		}

		Log.Info( $"{GameObject.Name} is attacking {closest.GameObject.Name}!" );
			if ( !Attack( closest ) )
			{
				//Log.Info( "Attacking failed" );
				Done = true;
			}
			else
			{
				Done = false;
			}
			return;
		
		
	}
	
	public override void SetNextTile()
	{
		
		base.SetNextTile();

		
	}



	public override void MoveTo(Tile destinationTile)
	{
		base.MoveTo(destinationTile);
		if(destinationTile == null)
		{
			Log.Warning( "Destination tile is null in MoveTo!" );
			return;
		}
	}

	

	private bool Attack(Unit target)
	{
		if ( TimeUnits < 4 )
		{
			//Log.Info( "Not enough Time Units to attack!" );	
			return false; // You can tweak TU cost for zombie melee
		}

		Log.Info( $"{GameObject.Name} attacks {target.GameObject.Name}!" );
		target.Hurt( 20 ); // Hardcoded melee damage
		DeductTUs( 4 );
		return true;
	}

	private List<Unit> FindEnemiesInRange(int radius)
	{
		var results = new List<Unit>();
		var boardUnits = BattleMaster.Instance.PlayerUnitsList;

		foreach ( var go in boardUnits )
		{
			var unit = go.Components.Get<Unit>();
			if ( unit == null || unit.Team == this.Team || unit.Health <= 0 ) continue;

			if ( Tile.Distance( this.CurrentTile, unit.CurrentTile ) <= radius )
			{
				results.Add( unit );
			}
		}

		return results;
	}

	private Unit GetClosestUnit(List<Unit> units)
	{
		Unit closest = null;
		float bestDist = float.MaxValue;

		foreach ( var unit in units )
		{
			float dist = Tile.Distance( this.CurrentTile, unit.CurrentTile );
			if ( dist < bestDist )
			{
				bestDist = dist;
				closest = unit;
			}
		}
		//Log.Info( "Returned closest unit " + closest );
		return closest;
	}

	
}






