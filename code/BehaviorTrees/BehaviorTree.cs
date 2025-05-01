
/*using Sandbox;
using Sandbox.Citizen;
using static Node;

public class BehaviorTree : Component
{
	[Property] NavMeshAgent Agent;
	[Property] GameObject gameObject;
	MoveToRand moveToRand;
	VisualSearch visSearch;
	Sequencer root = new Sequencer();
	Selector ActivatedSelector = new Selector();
	Selector PatrolSelector =  new Selector();
	Selector SeePlayerSelector = new Selector();
	Selector AttackPlayerSelector = new Selector();
	Sequencer testSequencer = new Sequencer();
	Activated distCheck;
	SeePlayer seePlayer;
	



	protected override void OnStart()
	{
		//Higher level Nodes will be executed top down (left to right).

		root.AddChild( ActivatedSelector );
		root.AddChild( PatrolSelector );
		root.AddChild( SeePlayerSelector );
		root.AddChild( AttackPlayerSelector );

		// Activation Condition/subtree

		// ActivatedSelector.AddChild(ActivatedCondition);

		 // Patrol subtree

		// PatrolSelector.AddChild(Con_PlayerNear) // This condition goes first, determines if the player is near.
		// If it is near, we can skip all these next nodes.
		
		//Sequencer moveRandSeq = new Sequencer(); // Define Subtree root Node
		//PatrolSelector.AddChild( moveRandSeq ); // Assign Subtree root node to parent intermediary node

		moveToRand = new MoveToRand( "Move to Random Point", "Test", gameObject, Agent ); // Define Subtree leaves
		visSearch = new VisualSearch("Random Angle Adjustment", "Test", gameObject, Agent);
		//randWait = new RandWait()
		//moveRandSeq.AddChild( moveToRand ); // Assign subtree leaves
		//moveRandSeq.AddChild( randWait ); /
		//moveRandSeq.AddChild( visSearch );

		// Search Subtree



		//VisualCheck

		//Sees Player Subtree. This also handles if the zombie loses LOS.

		GameObject player = Scene.Directory.FindByName( "Player" ).First();
		Log.Info( "Behavior Tree player valid? " + player.IsValid() );
		distCheck = new Activated( "Check Player distance", "Test", gameObject, Agent, player );
		seePlayer = new SeePlayer( "Check FOV", "Test", gameObject, Agent, player );


		testSequencer.AddChild( distCheck );
		testSequencer.AddChild( seePlayer );
		testSequencer.AddChild( moveToRand );
	






	}

	protected override void OnFixedUpdate()
	{
		//Log.Info( distCheck.Tick() );
		//Log.Info( visSearch.Tick() );
		//Log.Info( seePlayer.Tick() );
		//Log.Info( moveToRand.Tick() );
		testSequencer.Tick();


	}


}
*/
