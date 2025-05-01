
public class Selector : Node
{
	public override NodeState Tick()
	{
		foreach ( Node child in children )
		{
			var state = child.Tick();
			switch (state)
			{
				case NodeState.SUCCESS:
					State = state;
					return state;
				case NodeState.RUNNING:
					State = state;
					return state;
				case NodeState.FAILURE:
					break;
			}

		}
		State = NodeState.FAILURE;
		return State;
	}
}
