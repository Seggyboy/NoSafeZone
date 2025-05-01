public abstract class Node : Component
{
    public enum NodeState
    {
        SUCCESS,
        FAILURE,
        RUNNING,
    }

    protected List<Node> children = new List<Node>();
    public NodeState State { get; protected set; }

    public abstract NodeState Tick();

    public void AddChild(Node child)
    {
        children.Add(child);
		Log.Info("Child " + child + " Added to list. ");
		Log.Info( "List: " );
		foreach (var c in children )
		{
			Log.Info( c );
		}
    }

	public List<Node> GetChildren()
	{
		return children;
	}
}
