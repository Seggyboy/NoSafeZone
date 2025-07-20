using System;
using Sandbox;

public class AStarNode
{
	public Tile Tile;
	public AStarNode Parent;
	public float G; // Cost from start
	public float H; // Heuristic cost to end
	public float F => G + H; // Total cost

	public AStarNode( Tile tile )
	{
		Tile = tile;
	}
}




