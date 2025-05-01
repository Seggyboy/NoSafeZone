
using Sandbox;

public sealed class Behavior : Component
{
	[Property] BlackBoard blackboard;
	[Property] NavMeshAgent agent;
	Selector root;
	float LastTick = Time.Now;

	protected override void OnEnabled()
	{
		 root = new Selector();

		var Seq_Hunter = new Sequencer();
		root.AddChild( Seq_Hunter );
		var Con_PlayerClose = new RangeFinder( "Con_PlayerClose", "Checks if the player is in render range", blackboard, agent, 1000f );
		Seq_Hunter.AddChild( Con_PlayerClose );
		var Con_CanSee = new SeePlayer( "SeePlayer", "Checks if the player is in render range", blackboard, agent );
		Seq_Hunter.AddChild( Con_CanSee );
		var Sel_MoveOnPlayer = new Selector();
		Seq_Hunter.AddChild( Sel_MoveOnPlayer );
		var Con_PlayerInRange = new RangeFinder( "Con_PlayerInMeleeRange", "Checks if the player is in render range", blackboard, agent, 10f );
		Sel_MoveOnPlayer.AddChild(Con_PlayerInRange);
		var Act_MoveToPlayer = new MovePlayer( "MoveToPlayer", "Moves to the player", blackboard, agent );
		Sel_MoveOnPlayer.AddChild(Act_MoveToPlayer);
		//var Act_AttackPlayer = new AttackPlayer();
		//Seq_Hunter.AddChild( Act_AttackPlayer );




	}

	protected override void OnUpdate()
	{
		if (Time.Now - LastTick > 0.5)
		{
			root.Tick();
		}
		
	}
}
