
using System;
using Sandbox;

public class RangeFinder : ActionNode
{
	// This node gets distance between the agent and the player. returns success if the player is within a certain distance, failure if not.

	public RangeFinder( string name, string description, BlackBoard blackboard, NavMeshAgent agent, float distance ) : base( name, description, blackboard, agent )
	{
		this.distance = distance;
	}


	public float distance;



	public override NodeState Tick()
	{
		float distanceSquared = (Blackboard.PlayerLocation - Agent.WorldPosition).LengthSquared;
		float maxDist = distance * distance;
		//Log.Info(Blackboard.PlayerLocation+ " " + maxDist );
		if (  distanceSquared < maxDist  )
		{
			//Log.Info( Name + ' ' + "determined distance is met." );
			return NodeState.SUCCESS;
		}
		else
		{
			Log.Info( Name + ' ' + "determined distance is not met. Distance is: " +distanceSquared );
			return NodeState.FAILURE;
		}
	}
}






