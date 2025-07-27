using System;
using Sandbox;

public sealed class TravelerMap : Component
{

	[Property] public Traveler Traveler { get; set; } = null!; // Reference to the Traveler component
	[Property] public List<RegionNode> Nodes = new();
	[Property] public List<RegionNode> DestinationNodes = new();
	[Property] public int targetMiles = 1000; // Default target miles for the Traveler	
	private int CurrentNodeIndex { get; set; }
	
	[Property] public RegionNode CurrentNode;
	[Property] public RegionNode DestinationNode { get; set; } = null!;


	protected override void OnEnabled()
	{
		Traveler = Scene.GetAllComponents<Traveler>().FirstOrDefault();
		TravelerEvents.ReachedDestinationEvent += ReachedDestination;
		CurrentNode = Nodes[0];
		LoadFromFile();

		TravelerEvents.StartMoveConvoyEvent += TryStartMoving;
		TravelerEvents.StopMoveConvoyEvent += TryStopMoving;

	}

	protected override void OnDisabled()
	{
		TravelerEvents.ReachedDestinationEvent -= ReachedDestination;
		TravelerEvents.StartMoveConvoyEvent -= TryStartMoving;
		TravelerEvents.StopMoveConvoyEvent -= TryStopMoving;
		Log.Info( "TravelerMap: Disabled." );
	}
	protected override void OnUpdate()
	{
		
	}

	public void TryStartMoving()
	{
		if ( DestinationNode == null )
		{
			Log.Error( "TravelerMap: No destination set. Cannot start moving." );
			return;
		}
		
		//Log.Info( $"TravelerMap: Starting to move towards {DestinationNode.Name} at index {DestinationNode.Index}." );
		Traveler.StartMoving();
	}

	public void TryStopMoving()
	{
	

		//Log.Info( $"TravelerMap: Stopping movement towards {DestinationNode.Name} at index {DestinationNode.Index}." );
		Traveler.StopMoving();
	}

	public void ReachedDestination()
	{

		Log.Info( $"TravelerMap: Reached destination {DestinationNode.Name} at index {DestinationNode.Index}." );
		
		UpdateCurrentNode( DestinationNode.Index );

		DestinationNode = null;
	}

	[Button]
	public void SetDestination(RegionNode dest)
	{
		DestinationNode = dest;
		var dist = (int) PixelsToMiles(Vector2.DistanceBetween( CurrentNode.Position, DestinationNode.Position ));
		targetMiles = dist;

		Log.Info(dist + $"TravelerMap: Setting destination to {DestinationNode.Name} at distance {dist} miles. Distance between locations raw is: {Vector2.DistanceBetween( CurrentNode.Position, DestinationNode.Position )}" );
		Traveler.SetDistance( dist );

	}

	public void UpdateCurrentNode(int index)
	{
		if ( index < 0 || index >= Nodes.Count )
		{
			Log.Error( $"TravelerMap: Invalid node index {index}. Must be between 0 and {Nodes.Count - 1}." );
			return;
		}
		CurrentNodeIndex = index;
		CurrentNode =  Nodes[CurrentNodeIndex];
		DestinationNodes = CurrentNode.Neighbors
			.Select( n => Nodes.FirstOrDefault( node => node.Index == n ) )
			.Where( n => n != null )
			.ToList();
		Log.Info( $"TravelerMap: Current node updated to {CurrentNode.Name} at index {CurrentNodeIndex}." );
	}

	public RegionNode? GetNodeAtPosition( Vector2 pos, float zoom )
	{
		float radius = 10f;
		foreach ( var node in Nodes )
		{
			float dist = Vector2.DistanceBetween( pos, node.Position );
			if ( dist <= radius )
			{
				return node;
			}
		}
		return null;
	}


	public float PixelsToMiles( float pixels )
	{
		Log.Info("Multiplying " + pixels + " by " + GameConstants.Travel.PIXELSPERMILE );
		return pixels * GameConstants.Travel.PIXELSPERMILE;
	}




	[Button]
	public void LoadFromFile()
	{
		if ( !FileSystem.Data.FileExists( "regions.json" ) ) return;
		Nodes = Json.Deserialize<List<RegionNode>>( FileSystem.Data.ReadAllText( "regions.json" ) );
		Log.Info( $"Loaded {Nodes.Count} nodes." );
		CurrentNodeIndex = 0;
		UpdateCurrentNode( CurrentNodeIndex );

	}
	[Button]
	public void ClearNodes()
	{
		Nodes.Clear();
		DestinationNodes.Clear();
		CurrentNode = null;
		CurrentNodeIndex = 0;
		Log.Info( "TravelerMap: Cleared all nodes." );
	}	
}
