public class Sequencer : Node
{
	public override NodeState Tick()
	{
		foreach ( Node child in children )
		{
			NodeState childState = child.Tick();
			if ( NodeState.SUCCESS != childState )
			{
				State = childState;
				return childState;
			}

		}
		State = NodeState.SUCCESS;
		return State;
	}
}

