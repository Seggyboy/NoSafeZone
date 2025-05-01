
using System;
using System.Numerics;
using Sandbox;

public class MovePlayer: ActionNode
{
	// This node gets distance between the agent and the player. returns success if the player is within a certain distance, failure if not.

	public MovePlayer( string name, string description, BlackBoard blackboard, NavMeshAgent agent ) : base( name, description, blackboard, agent )
	{
	
	}






	public override NodeState Tick()
	{
		var ppos = Blackboard.PlayerLocation;
		var pvelocity = Blackboard.PlayerVelocity;
		float timeToReach = Agent.Velocity.Length / Vector3.DistanceBetween( Agent.WorldPosition, ppos );
		Vector3 predPos = ppos + pvelocity * timeToReach;
		//Log.Info( predPos );
		Agent.MoveTo( predPos );
		Log.Info( "Moving to: " + predPos );	

		if ( Vector3.DistanceBetween( Agent.WorldPosition, ppos ) < 30)
		{
			
			return NodeState.SUCCESS;
		}
		else
		{
			return NodeState.FAILURE;
		}
	}
}






