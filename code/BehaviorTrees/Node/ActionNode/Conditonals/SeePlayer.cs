
using System;
using Sandbox;

public class SeePlayer : ActionNode
{
	// This node performs a visual sweep between the current angle and finishing angle. 

	public SeePlayer( string name, string description, BlackBoard blackboard, NavMeshAgent agent) : base(name, description, blackboard, agent) 
	{
		
	}

	
	private Random rand = new Random();
	private float randomOffset;
	private Rotation currentRotation;
	private Rotation randomRotation;
	private bool turning = false;




	public override NodeState Tick()
	{
		Vector3 dirVector = (Blackboard.PlayerLocation - Agent.WorldPosition  ).Normal;
		Vector3 agentForward = Agent.WorldRotation.Forward;
		if ( Vector3.Dot(dirVector, agentForward) > MathF.Cos(60/2) )
		{
			Log.Info( Name + ' ' + "Player is in front of agent." );
			return NodeState.SUCCESS;
		}
		else
		{
			//Log.Info( Name + ' ' + "Player is not in front of agent." );
			return NodeState.FAILURE;
		}
	}
}






