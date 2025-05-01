public abstract class ActionNode : Node
{
	public string Name { get; protected set; }
	public string Description { get; protected set; }

	public BlackBoard Blackboard { get; protected set; }

	public NavMeshAgent Agent { get; protected set; }

	protected ActionNode( string name, string description, BlackBoard blackboard, NavMeshAgent agent )
	{
		Name = name;
		Description = description;
		Blackboard = blackboard;
		Agent = agent;
	}
	public override abstract NodeState Tick();
	


}

